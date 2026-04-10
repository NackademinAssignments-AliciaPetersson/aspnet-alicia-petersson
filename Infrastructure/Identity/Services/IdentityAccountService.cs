using Application.Abstractions.Identity;
using Application.Common.Outputs;
using Application.Common.Results;
using Domain.Exceptions.Custom;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public class IdentityAccountService(UserManager<AuthenticationUser> userManager) : IAccountService
{
    public async Task<Result<AuthenticationUserDetails?>> GetAuthenticationUserDetailsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new NullDomainException(nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<AuthenticationUserDetails?>.NotFound($"User with ID '{userId}' was not found");

        return Result<AuthenticationUserDetails?>.Ok(new AuthenticationUserDetails(user.Id, user.Email, user.PhoneNumber));
    }

    public async Task<Result> DeleteAuthenticationUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new NullDomainException(nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.NotFound($"User with ID '{userId}' was not found");

        var deleted = await userManager.DeleteAsync(user);
        return deleted.Succeeded
            ? Result.Ok()
            : Result.Error(deleted.Errors.FirstOrDefault()?.Description ?? "Unable to delete account");
    }
}
