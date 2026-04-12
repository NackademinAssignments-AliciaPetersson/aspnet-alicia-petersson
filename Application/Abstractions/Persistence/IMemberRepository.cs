using Domain.Abstractions.Persistence.Repositories;
using Domain.Aggregates.Members;

namespace Application.Abstractions.Persistence;

public interface IMemberRepository : IRepositoryBase<Member, string>
{
    Task<Member?> GetByUserIdAsync(string userId, CancellationToken ct = default);
}
