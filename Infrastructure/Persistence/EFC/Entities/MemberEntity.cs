using Domain.Abstractions.Persistence;
using Infrastructure.Identity;

namespace Infrastructure.Persistence.EFC.Entities;

public sealed class MemberEntity : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public byte[]? RowVersion { get; set; }

    //EFC Navigation props
    public AuthenticationUser? User { get; set; }
    public ICollection<MembershipEntity> Memberships { get; set; } = [];
}
