using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common.Results;
using Application.Modules.MembershipTypes.Inputs;
using Application.Modules.MembershipTypes.Outputs;
using Domain.Aggregates.Members.Entities;
using Domain.Exceptions.Custom;

namespace Application.Modules.MembershipTypes;

public sealed class MembershipTypeService(IMembershipTypeRepository membershipTypeRepo, IUnitOfWork uow) : IMembershipTypeService
{    
    public async Task<Result<MembershipTypeOutput?>> CreateMembershipTypeAsync(CreateMembershipTypeInput input, CancellationToken ct = default)
    {
        MembershipType membershipTypeInput;
        try
        {
            membershipTypeInput = MembershipType.Create(input.Name, input.BasePrice);
        }
        catch (ValidationDomainException ex)
        {
            return Result<MembershipTypeOutput?>.BadRequest(ex.Message);
        }

        var existing = await membershipTypeRepo.GetByMembershipNameAsync(input.Name, ct);
        if (existing is not null)
            return Result<MembershipTypeOutput?>.Conflict("A Membership type with the same name already exists");

        var created = await membershipTypeRepo.AddAsync(membershipTypeInput, ct);
        if (created is null)
            return Result<MembershipTypeOutput?>.Error();

        var saved = await uow.CommitAsync(ct);

        return saved > 0 ? Result<MembershipTypeOutput?>.Ok(ToOutput(created)) : Result<MembershipTypeOutput?>.Error("0 rows affected in database");
    }

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
