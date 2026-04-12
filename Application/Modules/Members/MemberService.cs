using Application.Abstractions.Identity;
using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common.Outputs;
using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Application.Modules.Members.Outputs;
using Domain.Abstractions.Logging;
using Domain.Aggregates.Members;
using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Application.Modules.Members;

public sealed class MemberService(IAuthService authService, ILogger logger, IMemberRepository memberRepo, IAccountService accountService, IUnitOfWork uow, IMembershipTypeRepository membershipTypeRepo) : IMemberService
{
    // -- MEMBER --
    public async Task<Result> CreateMemberAsync(CreateMemberInput input, CancellationToken ct = default)
    {
        if (input is null)
            return Result.BadRequest("input model must be provided");

        if (input.Email is null)
            return Result.BadRequest("Email must be provided");

        var existing = await authService.DoesUserExistAsync(input.Email);
        if (existing)
            return Result.Conflict("An account with the same email address already exists");

        Member? createdMember = null;
        await uow.ExecuteInTransactionAsync(async token =>
        {
            var registerResult = await authService.SignUpLocalUserAsync(input.Email, input.Password, "Member");
            if (!registerResult.Success)
            {
                logger.Log(registerResult.ErrorMessage ?? $"Unable to sign up member. User Id may be missing or invalid, user Id was: '{registerResult.Value}'");
                throw new ValidationDomainException(registerResult.ErrorMessage ?? "Unable to sign up member");
            }

            var userId = GuidValidator.EnsureValidGuid(registerResult.Value);

            var member = Member.Create(userId);

            createdMember = await memberRepo.AddAsync(member, token);
        }, ct);

        return createdMember is not null ? Result.Ok() : Result.Error("Could not create member");
    }

    public async Task<Result> CreateMemberForExternalUserAsync(CreateExternalMemberInput input, CancellationToken ct = default)
    {
        if (input is null)
            return Result.BadRequest("input model must be provided");

        if (input.Email is null)
            return Result.BadRequest("email must be provided");       

        var userId = GuidValidator.EnsureValidGuid(input.UserId);

        var member = Member.Create(userId, input.FirstName, input.LastName, input.ProfileImageUrl);

        Member? createdMember = await memberRepo.AddAsync(member, ct);
        await uow.CommitAsync(ct);

        return createdMember is not null ? Result.Ok() : Result.Error("Could not create member");
    }

    public async Task<Result> DeleteMemberAsync(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.BadRequest("UserId is missing");

        var member = await memberRepo.GetByUserIdAsync(userId, ct);
        if (member is null)
            return Result.NotFound($"Member with userId ID '{userId}' not found");

        await uow.ExecuteInTransactionAsync(async token =>
        {
            member.RemoveAllMemberships();
            await memberRepo.UpdateAsync(member.Id, member, ct);

            var memberDeleted = await memberRepo.RemoveByIdAsync(member.Id, ct);
            if (!memberDeleted)
            {
                logger.Log($"Member with Id '{member.Id}' was not able to be removed");
                throw new NotRemovedDomainException($"Member with Id '{member.Id}' was not removed");
            }

            var accountDeleted = await accountService.DeleteAuthenticationUserAsync(member.UserId);
            if (!accountDeleted.Success)
            {
                logger.Log($"AuthenticationUser with userId '{member.UserId}' was not able to be removed. Error: {accountDeleted.ErrorMessage}");
                throw new NotRemovedDomainException($"AuthenticationUser with userId '{member.UserId}' was not removed");
            }
        }, ct);

        return Result.Ok();
    }

    // -- MEMBER DETAILS --
    public async Task<Result<MemberDetails?>> GetMemberDetailsAsync(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new NullDomainException($"{nameof(userId)} cannot be null");

        var member = await memberRepo.GetByUserIdAsync(userId, ct);
        if (member is null)
            return Result<MemberDetails?>.NotFound($"Member with user Id '{userId}' was not found");

        var authUserResult = await accountService.GetAuthenticationUserDetailsAsync(userId);
        if (!authUserResult.Success)
            return Result<MemberDetails?>.NotFound($"Member with user Id '{userId}' was not found");

        var details = new MemberDetails(
            member.Id,
            member.UserId,
            authUserResult.Value?.Email,
            member.FirstName,
            member.LastName,
            authUserResult.Value?.PhoneNumber,
            member.ProfileImageUrl
        );

        return Result<MemberDetails?>.Ok(details);
    }

    public async Task<Result> UpdateMemberDetailsAsync(UpdateMemberDetailsInput details, CancellationToken ct = default)
    {
        if (details is null)
            throw new NullDomainException($"{nameof(details)} cannot be null");

        var member = await memberRepo.GetByUserIdAsync(details.UserId, ct);
        if (member is null)
            return Result.NotFound($"Member with user Id '{details.UserId}' was not found");

        member.UpdateDetailsInformation(details.FirstName, details.LastName, details.ImageUrl);

        await uow.ExecuteInTransactionAsync(async token =>
        {
            var updatedMember = await memberRepo.UpdateAsync(member.Id, member, ct);
            if (member is null)
                throw new NotUpdatedDomainException("Member could not be updated");

            var authUserInput = new UpdateAuthenticationUserDetailsInput(details.UserId, details.PhoneNumber);
            var authUserResult = await accountService.UpdateAuthenticationUserDetailsAsync(authUserInput);
            if (!authUserResult.Success)
                throw new NotUpdatedDomainException("Authentication User could not be updated");
        }, ct);

        return Result.Ok();
    }

    // -- MEMBERSHIPS
    public async Task<Result> SetMembershipAsync(SetMembershipInput membershipInput, CancellationToken ct = default)
    {
        if (membershipInput is null)
            return Result.BadRequest("input model must be provided");

        var member = await memberRepo.GetByUserIdAsync(membershipInput.UserId, ct);
        if (member is null)
            return Result.NotFound($"Member with UserId '{membershipInput.UserId}' not found");

        var membershipType = await membershipTypeRepo.GetByIdAsync(membershipInput.MembershipTypeId, ct);
        if (membershipType is null)
            return Result.NotFound($"membership Type with ID '{membershipInput.MembershipTypeId}' not found");

        try
        {
            member.AcquireMembership(membershipType);
            var updatedMember = await memberRepo.UpdateAsync(member.Id, member, ct);

            if (updatedMember is null)
                return Result.Error("Could not update Member");
        }
        catch (ValidationDomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch(Exception ex)
        {
            return Result.Error(ex.Message);
        }

        var saved = await uow.CommitAsync(ct);

        return saved > 0 ? Result.Ok() : Result.Error();
    }

    public async Task<Result<MembershipDetails?>> GetMembershipDetailsAsync(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result<MembershipDetails?>.BadRequest($"{nameof(userId)} cannot be null");

        var member = await memberRepo.GetByUserIdAsync(userId, ct);
        if (member is null)
            return Result<MembershipDetails?>.NotFound($"Member with user Id '{userId}' was not found");

        var currentMembership = member.CurrentMembership;

        var membershipInfo = currentMembership is null 
            ? null 
            : new ActiveMembership(currentMembership.Id, currentMembership.MembershipType.Name, DateOnly.FromDateTime(currentMembership.StartDateUtc), currentMembership.MonthlyPrice);

        var details = new MembershipDetails(member.Id, member.UserId, membershipInfo);

        return Result<MembershipDetails?>.Ok(details);
    }
}
