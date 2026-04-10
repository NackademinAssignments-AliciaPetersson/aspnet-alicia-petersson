using Domain.Abstractions.Persistence.Repositories;
using System.Linq.Expressions;

namespace Domain.Aggregates.Member;

public interface IMemberRepository : IRepositoryBase<Member, string>
{
    Task<Member?> GetByUserIdAsync(string userId, CancellationToken ct = default);
}
