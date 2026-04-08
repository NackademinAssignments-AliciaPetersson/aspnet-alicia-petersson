namespace Application.Modules.ContactRequests.Inputs;

public sealed record ContactRequestInput
(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string Message
);
