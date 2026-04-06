using Application.Common.Results;
using Application.Modules.Members.Inputs;
using Application.Modules.Members.Outputs;

namespace Application.Abstractions.Services;

public interface IMemberService
{
    Task<Result> CreateMemberAsync(CreateMemberInput input, CancellationToken ct = default);
}
