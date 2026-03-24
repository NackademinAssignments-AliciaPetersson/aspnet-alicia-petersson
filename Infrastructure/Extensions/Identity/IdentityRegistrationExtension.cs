using Infrastructure.Identity;
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
            options.LoginPath = "/auth/signin";
            options.LogoutPath = "/";
            options.AccessDeniedPath = "/error/401";

            options.Cookie.Name = "corefitness.identity.auth";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        return services;
    }
}
