using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public sealed record DateRange
{
    private DateRange()
    {
    }

    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }

    public TimeSpan Duration => EndUtc - StartUtc;

    
    public static Result<DateRange> Create(DateTime start, DateTime end)
    {
        var startUtc = ToUtc(start);
        var endUtc = ToUtc(end);

        if (endUtc <= startUtc)
            return BookingErrors.EndBeforeStart;

        return new DateRange { StartUtc = startUtc, EndUtc = endUtc };
    }

    
    public bool Overlaps(DateRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
