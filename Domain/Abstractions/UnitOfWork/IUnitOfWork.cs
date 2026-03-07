namespace Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct);
}
