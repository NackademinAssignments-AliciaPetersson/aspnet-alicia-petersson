using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EFC.Contexts;

public sealed class CoreFitnessContext(DbContextOptions<CoreFitnessContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreFitnessContext).Assembly);

    // Add Entity Db Sets below:
}
