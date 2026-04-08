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
}
