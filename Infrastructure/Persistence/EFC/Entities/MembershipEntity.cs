using Domain.Abstractions.Persistence;

namespace Infrastructure.Persistence.EFC.Entities;

public sealed class MembershipEntity : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string MemberId { get; set; } = null!;
    public int MembershipTypeId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public decimal MonthlyPrice { get; set; }

    //EFC Navigation props
    public MemberEntity? Member { get; set; }
    public MembershipTypeEntity? MembershipType { get; set; }
}
