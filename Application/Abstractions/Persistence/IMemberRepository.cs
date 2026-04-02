using Domain.Abstractions.Persistence.Repositories;

namespace Domain.Aggregates.Member;

public interface IMemberRepository : IRepositoryBase<Member, string>
{
}
