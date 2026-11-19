using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public static class RoomErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Rooms.NotFound", "The room does not exist.");

    public static readonly Error NameRequired =
        Error.Validation("Rooms.NameRequired", "Room name is required.");

    public static readonly Error InvalidCapacity =
        Error.Validation("Rooms.InvalidCapacity", "Capacity must be strictly positive.");
}
