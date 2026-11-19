using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Bookings.NotFound", "The booking does not exist.");

    public static readonly Error BookedByRequired =
        Error.Validation("Bookings.BookedByRequired", "BookedBy is required.");

    public static readonly Error EndBeforeStart =
        Error.Validation("Bookings.EndBeforeStart", "The end must be strictly after the start.");

    public static readonly Error InvalidAttendees =
        Error.Validation("Bookings.InvalidAttendees", "Attendees must be strictly positive.");

    public static readonly Error ExceedsCapacity =
        Error.Validation("Bookings.ExceedsCapacity", "The room does not have enough seats.");

    public static readonly Error StartInPast =
        Error.Validation("Bookings.StartInPast", "A booking cannot start in the past.");

    public static readonly Error TooLong =
        Error.Validation("Bookings.TooLong", "A booking can last at most 4 hours.");

    public static readonly Error RoomOccupied =
        Error.Conflict("Bookings.RoomOccupied", "The room is occupied in this interval.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Bookings.AlreadyCancelled", "The booking is already cancelled.");
}
