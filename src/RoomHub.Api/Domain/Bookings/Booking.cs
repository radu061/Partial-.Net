using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public sealed class Booking
{
    public const int MaxActiveBookingsPerDay = 2;                                // R9
    public static readonly TimeSpan MinCancelNotice = TimeSpan.FromMinutes(60);  // R11

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

    // All booking rules R2-R10 live here. "now" is a parameter (R5: from TimeProvider).
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        IEnumerable<Booking> activeBookingsOfRoom,
        IEnumerable<Booking> activeBookingsOfParticipant,
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

        if (range.Duration > room.MaxBookingDuration)                              // R6 + R10
            return BookingErrors.TooLong;

        if (activeBookingsOfRoom.Any(b => b.IsActive && b.Range.Overlaps(range)))  // R7
            return BookingErrors.RoomOccupied;

        var trimmedBookedBy = bookedBy.Trim();
        var sameDayCount = activeBookingsOfParticipant.Count(b =>                  // R9
            b.IsActive
            && b.BookedBy == trimmedBookedBy
            && b.Range.StartUtc.Date == range.StartUtc.Date);
        if (sameDayCount >= MaxActiveBookingsPerDay)
            return BookingErrors.DailyLimitExceeded;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = trimmedBookedBy,
            Range = range,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    // R8 + R11
    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (Range.StartUtc - nowUtc < MinCancelNotice)                             // R11 (exactly 60 min is ok)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
