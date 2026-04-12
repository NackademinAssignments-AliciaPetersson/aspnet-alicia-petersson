namespace Application.Modules.Members.Outputs;

public sealed record ActiveMembership(
    string Id,
    string Name,
    DateTime StartDate,
    decimal MonthlyPrice
);