namespace Application.Modules.Members.Inputs;

public sealed record UpdateMemberDetailsInput(
    string UserId,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    string? ImageUrl = null
);