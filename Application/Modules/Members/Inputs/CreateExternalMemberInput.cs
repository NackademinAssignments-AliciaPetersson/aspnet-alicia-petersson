namespace Application.Modules.Members.Inputs;

public sealed record CreateExternalMemberInput(string UserId, string? Email, string? FirstName = null, string? LastName = null, string? ProfileImageUrl = null);
