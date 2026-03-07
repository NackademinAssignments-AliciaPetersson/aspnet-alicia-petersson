namespace Domain.Abstractions.Persistence.Repositories;

public interface IRepositoryBase<TDomainModel, in TId>
{
    Task<TDomainModel?> AddAsync(TDomainModel model, CancellationToken ct  = default);
    Task<TDomainModel?> UpdateAsync(TId id, TDomainModel model, CancellationToken ct  = default);
    Task<bool> RemoveByIdAsync(TId id, CancellationToken ct  = default);
    Task<TDomainModel?> GetByIdAsync(TId id, CancellationToken ct  = default);
    Task<IReadOnlyList<TDomainModel>> GetAllAsync(CancellationToken ct  = default);
}
