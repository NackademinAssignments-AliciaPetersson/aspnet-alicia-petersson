namespace Application.Modules.Members.Outputs;

public sealed record ActiveMembership(
    string Id,
    string Name,
    DateOnly StartDate,
    decimal MonthlyPrice
);