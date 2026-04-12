using Domain.Abstractions.Persistence;

namespace Infrastructure.Persistence.EFC.Entities;

public sealed class MembershipTypeEntity : IEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
}
