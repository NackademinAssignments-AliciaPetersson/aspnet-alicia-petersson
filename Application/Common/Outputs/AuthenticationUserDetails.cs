namespace Application.Common.Outputs;

public sealed record AuthenticationUserDetails(string UserId, string? Email, string? PhoneNumber);
