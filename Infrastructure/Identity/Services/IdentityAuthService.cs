using Application.Abstractions.Identity;
using Application.Common.Results;
using Domain.Exceptions.Custom;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Services;

public sealed class IdentityAuthService(UserManager<AuthenticationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AuthenticationUser> signInManager) : IAuthService
{
    public async Task<bool> DoesUserExistAsync(string email) => await userManager.Users.AnyAsync(x => x.Email == email);

    public async Task<Result<string?>> SignUpLocalUserAsync(string email, string password, string? roleName = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new NullDomainException($"{nameof(email)} cannot be null");

        if (string.IsNullOrWhiteSpace(password))
            throw new NullDomainException($"{nameof(password)} cannot be null");

        var user = new AuthenticationUser
        {
            UserName = email,
            Email = email
        };

        var createdResult = await userManager.CreateAsync(user, password);
        if (!createdResult.Succeeded)
            return Result<string?>.Error(string.Join(", ", createdResult.Errors.Select(x => x.Description)));

        if (!string.IsNullOrWhiteSpace(roleName) && await roleManager.RoleExistsAsync(roleName))
            await userManager.AddToRoleAsync(user, roleName);        

        return Result<string?>.Ok(user.Id);
    }

    public async Task<Result> SignInLocalUserAsync(string email, string password, bool rememberMe = false)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result.BadRequest("Incorrect email address or password");

        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, false);

        if (result.IsNotAllowed)
            return Result.Error("This user is not allowed to login");

        if (result.RequiresTwoFactor)
            return Result.Error("This user requires two-factor authentication");

        if (!result.Succeeded)
            return Result.BadRequest("Incorrect email address or password");

        return Result.Ok();
    }

    public Task SignOutUserAsync() => signInManager.SignOutAsync();

    public async Task<bool> DeleteAccountAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new NullDomainException(nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var deleteResult = await userManager.DeleteAsync(user);
        return deleteResult.Succeeded;
    }
}
