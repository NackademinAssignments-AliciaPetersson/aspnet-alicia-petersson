using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Data;

internal static class IdentityInitializer
{
    public static async Task InitilizeDefaultRolesAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<IdentityRole>()
        {
            new("Admin"),
            new("Member"),
            new("Employee"),
        };

        try
        {
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role.Name) && !await roleManager.RoleExistsAsync(role.Name))
                    await roleManager.CreateAsync(role);
            }
        }
        catch { }
    }

    public static async Task InitilizeDefaultAdminAccountsAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthenticationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var admins = new List<string>()
        {
            "admin@domain.com",
        };

        try
        {
            if (!await userManager.Users.AnyAsync())
            {
                var defaultPassword = "BytMig123!";
                var defaultRoleName = "Admin";

                foreach (var admin in admins)
                {
                    var user = AuthenticationUser.Create(admin);
                    user.EmailConfirmed = true;

                    var created = await userManager.CreateAsync(user, defaultPassword);

                    if (created.Succeeded && await roleManager.RoleExistsAsync(defaultRoleName))
                        await userManager.AddToRoleAsync(user, defaultRoleName);
                }
            }
        }
        catch { }
    }
}
