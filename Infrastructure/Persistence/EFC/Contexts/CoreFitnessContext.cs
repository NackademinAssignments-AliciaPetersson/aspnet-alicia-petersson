using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EFC.Contexts;

public sealed class CoreFitnessContext(DbContextOptions<CoreFitnessContext> options) : IdentityDbContext<AuthenticationUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreFitnessContext).Assembly);
    }

    // Add Entity Db Sets below:
}
