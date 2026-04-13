using Domain.Aggregates.Members;
using Domain.Aggregates.Members.Entities;
using Domain.Aggregates.WorkoutSessions.Entities;
using Domain.Common.Validators;
using Domain.Exceptions.Custom;

namespace Domain.Aggregates.WorkoutSessions;

public sealed class WorkoutSession
{
    private readonly List<Booking> _bookings = [];
    private readonly TimeSpan _cancellationDeadlineBeforeStart;

    private WorkoutSession(string id, string title, DateTime startTime, DateTime endTime, string employeeId, int maxParticipants, int cancellationDeadlineBeforeStartHours, List<Booking>? bookings = null)
    {
        Id = GuidValidator.EnsureValidGuid(id);
        Title = StringValidation.Required(title, nameof(title));
        EmployeeId = GuidValidator.EnsureValidGuid(employeeId);

        if (startTime > endTime)
            throw new ValidationDomainException("Start time cannot not be after end time");
        StartTime = startTime;
        EndTime = endTime;

        if (maxParticipants > 1)
            throw new ValidationDomainException("Max participants must be a positive value");
        MaxParticipants = maxParticipants;

        _bookings = bookings is null ? [] : bookings;
        _cancellationDeadlineBeforeStart = TimeSpan.FromHours(cancellationDeadlineBeforeStartHours);
    }

    public string Id { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string EmployeeId { get; private set; } = null!;
    public int MaxParticipants { get; private set; }
    public bool IsFullyBooked => _bookings.Count >= MaxParticipants;
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();
    //public string CategoryId { get; private set; } = null!;

    public static WorkoutSession Create(string title, DateTime startTime, DateTime endTime, string employeeId, int maxParticipants, int cancellationDeadlineBeforeStartHours = 3)
    {
        return new(Guid.NewGuid().ToString(), title, startTime, endTime, employeeId, maxParticipants, cancellationDeadlineBeforeStartHours);
    }
    public static WorkoutSession Rehydrate(string id, string title, DateTime startTime, DateTime endTime, string employeeId, int maxParticipants, int cancellationDeadlineBeforeStartHours, List<Booking>? bookings)
    {
        return new(id, title, startTime, endTime, employeeId, maxParticipants, cancellationDeadlineBeforeStartHours, bookings);
    }

    public void AddBooking(string memberId)
    {
        if (string.IsNullOrWhiteSpace(memberId))
            throw new ValidationDomainException("Member Id is required");

        if (_bookings.Count >= MaxParticipants)
            throw new ValidationDomainException("Workout session is fully booked");

        if (_bookings.Any(b => b.MemberId == memberId))
            throw new ValidationDomainException("Member has already booked this workout session");

        var booking = Booking.Create(memberId);
        _bookings.Add(booking);
    }

    public void RemoveBooking(string memberId) 
    {
        if (string.IsNullOrWhiteSpace(memberId))
            throw new ValidationDomainException("Member Id is required");

        var booking = _bookings.FirstOrDefault(b => b.MemberId == memberId) 
            ?? throw new ValidationDomainException("Member has not booked this workout session");

        if (DateTime.UtcNow > StartTime - _cancellationDeadlineBeforeStart)
            throw new ValidationDomainException("Cannot cancel booking this close to the session start");

        _bookings.Remove(booking);
    }
}
