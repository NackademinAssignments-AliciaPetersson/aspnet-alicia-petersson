using Application.Abstractions.Identity;
using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Domain.Exceptions.Custom;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        var existing = await DoesUserExistAsync(email);
        if (existing)
            return Result<string?>.Conflict("An account with the same email address already exists");

        var user = AuthenticationUser.Create(email);

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

    public async Task<Result<CreateExternalMemberInput?>> SignInExternalMemberAsync(string roleName = "Member")
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return Result<CreateExternalMemberInput?>.Error("Could not load external login information");

        var signInResult = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );

        if (signInResult.Succeeded)
            return Result<CreateExternalMemberInput?>.Ok(null);

        //User does not exist - creating AuthenticationUser
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Result<CreateExternalMemberInput?>.Error($"No email address was returned from {info.LoginProvider}");

        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var imageUrl = info.LoginProvider switch
        {
            "GitHub" => info.Principal.FindFirstValue("urn:github:avatar"),
            _ => "~/images/default_profile_image.png"
        };        

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = AuthenticationUser.Create(email);

            var createdResult = await userManager.CreateAsync(user);
            if (!createdResult.Succeeded)
                return Result<CreateExternalMemberInput?>.Error(string.Join(", ", createdResult.Errors.Select(x => x.Description)));

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleResult = await userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                    return Result<CreateExternalMemberInput?>.Error(string.Join(", ", roleResult.Errors.Select(x => x.Description)));
            }
        }

        //Add external login information to user
        var loginResult = await userManager.AddLoginAsync(user, info);
        if (!loginResult.Succeeded && loginResult.Errors.All(x => x.Code != "LoginAlreadyAssociated"))
            return Result<CreateExternalMemberInput?>.Error(string.Join(", ", loginResult.Errors.Select(x => x.Description)));

        await signInManager.SignInAsync(user, isPersistent: false);

        return Result<CreateExternalMemberInput?>.Ok(new CreateExternalMemberInput(user.Id, email, firstName, lastName, imageUrl));
    }
}