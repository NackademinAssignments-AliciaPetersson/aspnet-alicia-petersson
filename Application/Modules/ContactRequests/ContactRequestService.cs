using Application.Abstractions.Persistence;
using Application.Abstractions.Services;
using Application.Common.Results;
using Application.Modules.ContactRequests.Inputs;
using Application.Modules.ContactRequests.Outputs;
using Domain.Abstractions.Logging;
using Domain.Aggregates.ContactRequest;
using System.Diagnostics;

namespace Application.Modules.ContactRequests;

public sealed class ContactRequestService(IContactRequestRepository repo, ILogger logger, IUnitOfWork uow) : IContactRequestService
{
    public async Task<Result> CreateContactRequestAsync(ContactRequestInput input, CancellationToken ct = default)
    {
        try
        {
            if (input is null)
            {
                return Result.Error("input model must be provided");
            }

            ContactRequest? model = ContactRequest.Create(
                input.FirstName,
                input.LastName,
                input.Email,
                input.PhoneNumber,
                input.Message
            );

            await repo.AddAsync(model, ct);

            await uow.CommitAsync(ct);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
            logger.Log(ex);

            return Result.Error();
        }
    }

    public async Task<Result> DeleteContactRequestAsync(string id, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return Result.Error("Id must be provided");

            ContactRequest? model = await repo.GetByIdAsync(id, ct);
            if (model is null)
                return Result.NotFound($"Contact Request with id '{id}' was not found.");

            var deleted = await repo.RemoveByIdAsync(model.Id, ct);

            if (deleted)
                await uow.CommitAsync(ct);

            return deleted ? Result.Ok() : Result.Error();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
            logger.Log(ex);

            return Result.Error();
        }
    }

    public async Task<Result<ContactRequestOutput?>> GetContactRequestByIdAsync(string id, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return Result<ContactRequestOutput?>.Error("Id must be provided");

            ContactRequest? model = await repo.GetByIdAsync(id, ct);
            return model is null
                ? Result<ContactRequestOutput?>.NotFound($"Contact Request with id '{id}' was not found.")
                : Result<ContactRequestOutput?>.Ok(ToOutput(model));

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
            logger.Log(ex);

            return Result<ContactRequestOutput?>.Error();
        }
    }

    public async Task<Result<IReadOnlyList<ContactRequestOutput>>> GetContactRequestsAsync(CancellationToken ct = default)
    {
        try
        {
            var models = await repo.GetAllAsync(ct);
            var contactRequestOutputs = models.Select(cr => ToOutput(cr)).ToList();
            return Result<IReadOnlyList<ContactRequestOutput>>.Ok(contactRequestOutputs);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Console.WriteLine(ex);
            logger.Log(ex);

            return Result<IReadOnlyList<ContactRequestOutput>>.Error();
        }
    }

    private static ContactRequestOutput ToOutput(ContactRequest model)
    {
        var output = new ContactRequestOutput(model.Id, $"{model.FirstName} {model.LastName}", model.PhoneNumber, model.Message);
        return output;
    }
}