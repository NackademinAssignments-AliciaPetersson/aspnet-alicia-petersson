using Domain.Abstractions.Logging;
using Domain.Aggregates.Member;
using Infrastructure.Persistence.EFC.Contexts;
using Infrastructure.Persistence.EFC.Entities;

namespace Infrastructure.Persistence.EFC.Repositories;

public class MemberRepository(CoreFitnessContext context, ILogger logger) : RepositoryBase<Member, string, MemberEntity, CoreFitnessContext>(context, logger), IMemberRepository
{
    protected override void ApplyUpdates(Member model, MemberEntity entity)
    {
        throw new NotImplementedException();
    }

    protected override Member ToDomainModel(MemberEntity entity)
    {
        var model = Member.Rehydrated(entity.Id, entity.UserId, entity.FirstName, entity.LastName, entity.ProfileImageUrl);
        return model;
    }

    protected override MemberEntity ToEntity(Member model)
    {
        var entity = new MemberEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            ProfileImageUrl = model.ProfileImageUrl,
        };
        return entity;
    }
}
