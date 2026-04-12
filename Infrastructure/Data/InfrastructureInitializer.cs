using Infrastructure.Identity.Data;
using Infrastructure.Persistence.EFC.Contexts;
using Infrastructure.Persistence.EFC.Data;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Data;

public static class InfrastructureInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IHostEnvironment environment)
    {
        await PersistenceInitializer.InitializeDatabaseAsync(serviceProvider, environment);

        await IdentityInitializer.InitilizeDefaultRolesAsync(serviceProvider);

        await IdentityInitializer.InitilizeDefaultAdminAccountsAsync(serviceProvider);

        await ContextInitilizer.InitilizeDefaultMembershipTypes(serviceProvider);
    }
}