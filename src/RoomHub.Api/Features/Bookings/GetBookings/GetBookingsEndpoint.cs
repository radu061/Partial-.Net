using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public sealed record Response(
        Guid Id, Guid RoomId, string BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees, string Status);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/bookings/{id:guid}", Handle).WithTags("Bookings").WithName("GetBooking");

    private static IResult Handle(Guid id, BookingService service)
    {
        var b = service.Get(id);
        return Results.Ok(new Response(
            b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
    }
}