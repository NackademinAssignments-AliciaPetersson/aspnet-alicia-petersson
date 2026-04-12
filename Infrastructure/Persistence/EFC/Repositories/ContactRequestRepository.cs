using Application.Abstractions.Persistence;
using Domain.Abstractions.Logging;
using Domain.Aggregates.ContactRequest;
using Infrastructure.Persistence.EFC.Contexts;
using Infrastructure.Persistence.EFC.Entities;

namespace Infrastructure.Persistence.EFC.Repositories;

public sealed class ContactRequestRepository(CoreFitnessContext context, ILogger logger) : RepositoryBase<ContactRequest, string, ContactRequestEntity, CoreFitnessContext>(context, logger), IContactRequestRepository
{
    protected override void ApplyUpdates(ContactRequest model, ContactRequestEntity entity)
    {
        throw new NotImplementedException();
    }

    protected override ContactRequest ToDomainModel(ContactRequestEntity entity)
    {
        var model = ContactRequest.Rehydrate(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.PhoneNumber, entity.Message, entity.CreatedAtUtc);
        return model;
    }

    protected override ContactRequestEntity ToEntity(ContactRequest model)
    {
        var entity = new ContactRequestEntity()
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Message = model.Message,
            CreatedAtUtc = model.CreatedAtUtc
        };

        return entity;
    }
}
