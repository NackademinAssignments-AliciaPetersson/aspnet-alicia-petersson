using Domain.Common.Validators;

namespace Domain.Aggregates.ContactRequest;

public class ContactRequest
{
    private ContactRequest(string id, string firstName, string lastName, string email, string? phoneNumber, string message, DateTime createdAtUtc)
    {
        Id = GuidValidator.EnsureValidGuid(id);
        FirstName = StringValidation.Required(firstName, nameof(firstName));
        LastName = StringValidation.Required(lastName, nameof(lastName));
        Email = EmailValidation.Validate(email, nameof(email));
        PhoneNumber = phoneNumber;
        Message = StringValidation.Required(message, nameof(message));
        CreatedAtUtc = createdAtUtc;
    }

    public string Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string Message { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public static ContactRequest Create(string firstName, string lastName, string email, string? phoneNumber, string message)
        => new(Guid.NewGuid().ToString(), firstName, lastName, email, phoneNumber, message, DateTime.UtcNow);

    public static ContactRequest Rehydrate(string id, string firstName, string lastName, string email, string? phoneNumber, string message, DateTime createdAt)
        => new(id, firstName, lastName, email, phoneNumber, message, createdAt);
}
