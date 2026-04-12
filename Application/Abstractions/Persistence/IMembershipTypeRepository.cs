using Domain.Abstractions.Persistence.Repositories;
using Domain.Aggregates.Members.Entities;

namespace Application.Abstractions.Persistence;

public interface IMembershipTypeRepository : IRepositoryBase<MembershipType, int>
{
    Task<MembershipType?> GetByMembershipNameAsync(string name, CancellationToken ct = default);
}
