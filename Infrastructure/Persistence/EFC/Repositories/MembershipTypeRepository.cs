using Application.Abstractions.Persistence;
using Domain.Abstractions.Logging;
using Domain.Aggregates.Members;
using Domain.Aggregates.Members.Entities;
using Infrastructure.Persistence.EFC.Contexts;
using Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EFC.Repositories;

public class MembershipTypeRepository(CoreFitnessContext context, ILogger logger) : RepositoryBase<MembershipType, int, MembershipTypeEntity, CoreFitnessContext>(context, logger), IMembershipTypeRepository
{
    public async Task<MembershipType?> GetByMembershipNameAsync(string name, CancellationToken ct = default)
    {
        var entity = await Set.AsNoTracking().SingleOrDefaultAsync(e => e.Name == name, ct);
        return entity is null ? default : ToDomainModel(entity);
    }

    protected override void ApplyUpdates(MembershipType model, MembershipTypeEntity entity)
    {
        throw new NotImplementedException();
    }

    protected override MembershipType ToDomainModel(MembershipTypeEntity entity)
    {
        var model = MembershipType.Rehydrate(entity.Id, entity.Name, entity.BasePrice, entity.IsActive);
        return model;
    }

    protected override MembershipTypeEntity ToEntity(MembershipType model)
    {
        var entity = new MembershipTypeEntity
        {
            Id = model.Id,
            Name = model.Name,
            BasePrice = model.BasePrice,
            IsActive = model.IsActive,
        };
        return entity;
    }
}
