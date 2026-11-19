using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public sealed record Request(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

    public sealed record Response(Guid Id, Guid RoomId, string Status);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/rooms/{roomId:guid}/bookings", Handle).WithTags("Bookings").WithName("CreateBooking");

    private static IResult Handle(Guid roomId, Request request, BookingService service)
    {
        var booking = service.Create(roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);
        return Results.Created(
            $"/api/bookings/{booking.Id}",
            new Response(booking.Id, booking.RoomId, booking.Status.ToString()));
    }
}