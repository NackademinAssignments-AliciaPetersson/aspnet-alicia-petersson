namespace Application.Modules.ContactRequests.Outputs;

public sealed record ContactRequestOutput
(
    string Name,
    string Email,
    string? PhoneNumber,
    string Message
);
