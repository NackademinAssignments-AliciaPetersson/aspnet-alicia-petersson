using Application.Common.Results;
using Application.Modules.ContactRequests.Inputs;
using Application.Modules.ContactRequests.Outputs;
using Domain.Aggregates.ContactRequest;

namespace Application.Abstractions.Services;

public interface IContactRequestService
{
    Task<Result> CreateContactRequestAsync(ContactRequestInput input, CancellationToken ct = default);
    Task<Result> DeleteContactRequestAsync(string id, CancellationToken ct = default);

    Task<Result<ContactRequestOutput?>> GetContactRequestByIdAsync(string id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ContactRequestOutput>>> GetContactRequestsAsync(CancellationToken ct = default);
}
