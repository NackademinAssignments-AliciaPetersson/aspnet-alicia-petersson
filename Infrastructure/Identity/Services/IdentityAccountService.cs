using Application.Abstractions.Identity;
using Application.Common.Results;
using Domain.Exceptions.Custom;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public class IdentityAccountService(UserManager<AuthenticationUser> userManager) : IAccountService
{
    public async Task<Result> RemoveMemberAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new NullDomainException(nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.NotFound($"User with ID '{userId}' was noot found");

        var deleted = await userManager.DeleteAsync(user);
        return deleted.Succeeded
            ? Result.Ok()
            : Result.Error(deleted.Errors.FirstOrDefault()?.Description ?? "Unable to delete account");
    }
}
