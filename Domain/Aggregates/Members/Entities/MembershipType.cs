using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Domain.Aggregates.Members.Entities;

public sealed class MembershipType {
    private MembershipType(int id, string name, decimal basePrice, bool isActive)
    {
        Id = id;
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
}