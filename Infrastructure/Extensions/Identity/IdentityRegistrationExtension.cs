using Application.Abstractions.Identity;
using Infrastructure.Identity;
using Infrastructure.Identity.Services;
using Infrastructure.Persistence.EFC.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.Identity;

public static class IdentityRegistrationExtension
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AuthenticationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<CoreFitnessContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/sign-in";
            options.LogoutPath = "/";
            options.AccessDeniedPath = "/error/401";

            options.Cookie.Name = "corefitness.identity.auth";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddScoped<IAccountService, IdentityAccountService>();

        return services;
    }
}
