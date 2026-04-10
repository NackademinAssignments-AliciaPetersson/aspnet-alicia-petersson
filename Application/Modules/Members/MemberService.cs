using Application.Abstractions.Identity;
using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Domain.Abstractions.Logging;
using Domain.Aggregates.Member;
using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Application.Modules.Members;

public sealed class MemberService(IAuthService authService, ILogger logger, IMemberRepository memberRepo, IUnitOfWork uow) : IMemberService
{
    public async Task<Result> CreateMemberAsync(CreateMemberInput input, CancellationToken ct = default)
    {        
        if (input is null)
            return Result.BadRequest("input model must be provided");

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

        return Result.Ok();        
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
            var memberDeleted = await memberRepo.RemoveByIdAsync(member.Id, ct);
            if (!memberDeleted)
            {
                logger.Log($"Member with Id '{member.Id}' was not able to be removed");
                throw new NotRemovedDomainException($"Member with Id '{member.Id}' was not removed");
            }

            var accountDeleted = await authService.DeleteAccountAsync(member.UserId);
            if (!accountDeleted)
            {
                logger.Log($"AuthenticationUser with userId '{member.UserId}' was not able to be removed");
                throw new NotRemovedDomainException($"AuthenticationUser with userId '{member.UserId}' was not removed");
            }
        }, ct);

        return Result.Ok();
    }
}
