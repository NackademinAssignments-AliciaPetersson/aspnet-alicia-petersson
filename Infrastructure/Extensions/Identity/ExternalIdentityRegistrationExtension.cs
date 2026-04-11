using AspNet.Security.OAuth.GitHub;
using Infrastructure.Identity.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructure.Extensions.Identity;

public static class ExternalIdentityRegistrationExtension
{
    public static IServiceCollection AddExternalIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationBuilder = services.AddAuthentication();

        var githubOptions = configuration.GetSection(GitHubAuthOptions.SectionName).Get<GitHubAuthOptions>();

        if (githubOptions is not null && !string.IsNullOrWhiteSpace(githubOptions.ClientId) && !string.IsNullOrWhiteSpace(githubOptions.ClientSecret))
        {
            authenticationBuilder.AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = githubOptions.ClientId;
                options.ClientSecret = githubOptions.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.CallbackPath = "/signin-github";

                options.Scope.Add("user:email");

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey("urn:github:login", "login");
                options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");
                options.ClaimActions.MapJsonKey("urn:github:url", "html_url");

                options.SaveTokens = true;
            });
        }

        return services;
    }
}
