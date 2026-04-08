using Domain.Abstractions.Persistence;

namespace Infrastructure.Persistence.EFC.Entities;

public sealed class ContactRequestEntity : IEntity<string>
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}
