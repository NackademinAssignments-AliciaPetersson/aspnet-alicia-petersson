namespace Application.Modules.Members.Outputs;

public sealed record MemberDetails(
    string Id,
    string UserId,
    string? Email,
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    string? ImageUrl = null
);
