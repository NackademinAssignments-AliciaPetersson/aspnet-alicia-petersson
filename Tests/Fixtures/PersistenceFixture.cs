using Infrastructure.Persistence.EFC.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Fixtures;

public sealed class PersistenceFixture : IAsyncLifetime
{
    private SqliteConnection? _conn;
    public DbContextOptions<CoreFitnessContext> Options { get; set; } = default!;
    public CoreFitnessContext CreateContext() => new(Options);

    public async Task DisposeAsync()
    {
        if (_conn is not null)
        {
            await _conn.CloseAsync();
            await _conn.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        _conn = new SqliteConnection("Data Source=:memory:;");
        await _conn.OpenAsync();

        Options = new DbContextOptionsBuilder<CoreFitnessContext>().UseSqlite(_conn).Options;

        await using var context = new CoreFitnessContext(Options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class PersistenceCollection : ICollectionFixture<PersistenceFixture>
{
    public const string Name = "Persistence";
}
