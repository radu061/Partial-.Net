namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Booking>> GetActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);

    // R9: active bookings of one participant, across all rooms.
    Task<IReadOnlyList<Booking>> GetActiveForParticipantAsync(string bookedBy, CancellationToken cancellationToken);

    void Add(Booking booking);
}
