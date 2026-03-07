using Infrastructure.Persistence.EFC.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Persistence.EFC;

public static class PersistenceInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IHostEnvironment environment, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(environment);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoreFitnessContext>();


        if (environment.IsDevelopment())
        {
            await context.Database.EnsureCreatedAsync(ct);
        }
        else
        {
            await context.Database.MigrateAsync(ct);
        }
    }
}
