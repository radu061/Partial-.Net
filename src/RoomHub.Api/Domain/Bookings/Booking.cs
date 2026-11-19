using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateRange Range { get; private set; } = null!;
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;
    
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        IEnumerable<Booking> activeBookingsOfRoom,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))                                   // R2
            return BookingErrors.BookedByRequired;

        var rangeResult = DateRange.Create(startUtc, endUtc);                      // R3
        if (rangeResult.IsFailure)
            return rangeResult.Error;
        var range = rangeResult.Value;

        if (attendees <= 0)                                                        // R4
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)                                             // R4
            return BookingErrors.ExceedsCapacity;

        if (range.StartUtc < nowUtc)                                               // R5
            return BookingErrors.StartInPast;

        if (range.Duration > MaxDuration)                                          // R6 (exactly 4h is ok)
            return BookingErrors.TooLong;

        if (activeBookingsOfRoom.Any(b => b.IsActive && b.Range.Overlaps(range)))  // R7
            return BookingErrors.RoomOccupied;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy.Trim(),
            Range = range,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    
    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
