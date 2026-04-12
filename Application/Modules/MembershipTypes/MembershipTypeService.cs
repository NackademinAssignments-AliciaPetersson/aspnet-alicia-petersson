using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common.Results;
using Application.Modules.MembershipTypes.Outputs;
using Domain.Aggregates.Members.Entities;

namespace Application.Modules.MembershipTypes;

public sealed class MembershipTypeService(IMembershipTypeRepository membershipTypeRepo) : IMembershipTypeService
{
    public async Task<Result<MembershipTypeOutput?>> GetMembershipTypeByIdAsync(int id, CancellationToken ct = default)
    {
        if (id < 0)
            return Result<MembershipTypeOutput?>.BadRequest($"Id '{id}' is invalid");

        var membershipType = await membershipTypeRepo.GetByIdAsync(id, ct);

        return membershipType is null ? Result<MembershipTypeOutput?>.NotFound("Membership type not found") : Result<MembershipTypeOutput?>.Ok(ToOutput(membershipType));
    }

    public async Task<Result<IReadOnlyList<MembershipTypeOutput>>> GetMembershipTypesAsync(CancellationToken ct = default)
    {
        var membershipTypes = await membershipTypeRepo.GetAllAsync(ct);
        var outputs = membershipTypes.Select(membershipType => ToOutput(membershipType)).ToList();
        return Result<IReadOnlyList<MembershipTypeOutput>>.Ok(outputs);
    }

    private static MembershipTypeOutput ToOutput(MembershipType model)
    {
        var output = new MembershipTypeOutput(model.Id, model.Name, model.BasePrice, model.IsActive);        

        return output;
    }
}
