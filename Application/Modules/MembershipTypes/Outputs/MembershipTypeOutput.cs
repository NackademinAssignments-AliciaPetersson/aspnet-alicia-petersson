namespace Application.Modules.MembershipTypes.Outputs;

public sealed record MembershipTypeOutput(int Id, string Name, decimal BasePrice, bool IsActive);
