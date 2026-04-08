using Domain.Abstractions.Persistence.Repositories;
using Domain.Aggregates.ContactRequest;

namespace Application.Abstractions.Persistence;

public interface IContactRequestRepository : IRepositoryBase<ContactRequest, string>
{

}
