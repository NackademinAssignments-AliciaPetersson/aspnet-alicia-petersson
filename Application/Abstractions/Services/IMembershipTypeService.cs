using Application.Common.Results;
using Application.Modules.MembershipTypes.Outputs;

namespace Application.Abstractions.Services;

public interface IMembershipTypeService
{
    Task<Result<MembershipTypeOutput?>> GetMembershipTypeByIdAsync(int id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<MembershipTypeOutput>>> GetMembershipTypesAsync(CancellationToken ct = default);
}
