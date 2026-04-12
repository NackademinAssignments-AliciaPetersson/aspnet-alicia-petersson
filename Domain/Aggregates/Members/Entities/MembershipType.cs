using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Domain.Aggregates.Members.Entities;

public sealed class MembershipType {
    private MembershipType(int id, string name, decimal basePrice, bool isActive)
    {
        if (id < 0)
            throw new ValidationDomainException($"Id '{id}' not valid.");

        Id = id;
        Name = StringValidation.Required(name, "MembershipType Name");
        BasePrice = PriceValidation.IsNotNegative(basePrice, nameof(basePrice));
        IsActive = isActive;
    }

    private MembershipType(string name, decimal basePrice, bool isActive)
    {
        Name = StringValidation.Required(name, "MembershipType Name");
        BasePrice = PriceValidation.IsNotNegative(basePrice, nameof(basePrice));
        IsActive = isActive;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal BasePrice { get; private set; }
    public bool IsActive { get; private set; }

    public static MembershipType Rehydrate(int id, string name, decimal price, bool IsActive) 
    {
        return new MembershipType(id, name, price, IsActive);
    }
    public static MembershipType Create(string name, decimal price) 
    {
        return new MembershipType(name, price, true);
    }
}