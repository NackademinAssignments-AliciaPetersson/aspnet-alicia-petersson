namespace Application.Modules.Members.Outputs;

public sealed record MembershipDetails(
    string Id,
    string UserId,
    ActiveMembership? ActiveMembership
);