using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Application.Modules.Members.Outputs;

namespace Application.Abstractions.Services;

public interface IMemberService
{
    Task<Result> CreateMemberAsync(CreateMemberInput input, CancellationToken ct = default);
    Task<Result> CreateMemberForExternalUserAsync(CreateExternalMemberInput input, CancellationToken ct = default);
    Task<Result<MemberDetails?>> GetMemberDetailsAsync(string userId, CancellationToken ct = default);
    Task<Result> UpdateMemberDetailsAsync(UpdateMemberDetailsInput details, CancellationToken ct = default);
    Task<Result> SetMembershipAsync(SetMembershipInput membershipInput, CancellationToken ct = default);
    Task<Result<MembershipDetails?>> GetMembershipDetailsAsync(string userId, CancellationToken ct = default);
    Task<Result> DeleteMemberAsync(string userId, CancellationToken ct = default);
}
