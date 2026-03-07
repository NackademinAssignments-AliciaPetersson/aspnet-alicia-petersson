using Application.Abstractions.Persistence;
using Infrastructure.Persistence.EFC.Contexts;

namespace Infrastructure.Persistence.EFC.UnitOfWork;

public sealed class UnitOfWork(CoreFitnessContext context) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        try
        {
            await action(ct);
            await context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
