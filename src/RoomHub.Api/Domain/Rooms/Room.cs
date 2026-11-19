using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

public sealed class Room
{
    public const int LargeRoomThreshold = 10;
    public static readonly TimeSpan MaxBookingForSmallRoom = TimeSpan.FromHours(4);  // R6
    public static readonly TimeSpan MaxBookingForLargeRoom = TimeSpan.FromHours(2);  // R10

    private Room()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    // R6 + R10: capacity > 10 -> 2 hours, otherwise 4 hours (exactly the limit is allowed).
    public TimeSpan MaxBookingDuration =>
        Capacity > LargeRoomThreshold ? MaxBookingForLargeRoom : MaxBookingForSmallRoom;

    // R1
    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.NameRequired;

        if (capacity <= 0)
            return RoomErrors.InvalidCapacity;

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }
}
