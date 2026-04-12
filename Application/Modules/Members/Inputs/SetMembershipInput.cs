namespace Application.Modules.Members.Inputs;

public sealed record SetMembershipInput(
    string UserId,
    int MembershipTypeId
);