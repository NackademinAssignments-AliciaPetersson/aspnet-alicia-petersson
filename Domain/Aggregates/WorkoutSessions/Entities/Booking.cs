using Domain.Common.Validators;

namespace Domain.Aggregates.WorkoutSessions.Entities;

public sealed class Booking
{
    private Booking(string id, string memberId, DateTime bookingTime)
    {
        Id = GuidValidator.EnsureValidGuid(id);
        MemberId = GuidValidator.EnsureValidGuid(memberId);
        BookingTime = bookingTime;
    }

    public string Id { get; private set; } = null!;
    public string MemberId { get; private set; } = null!;
    public DateTime BookingTime { get; private set; }

    public static Booking Create(string memberId)
    {
        return new(Guid.NewGuid().ToString(), memberId, DateTime.UtcNow);
    }
    public static Booking Rehydrate(string id, string memberId, DateTime bookingTime)
    {
        return new(id, memberId, bookingTime);
    }
}
