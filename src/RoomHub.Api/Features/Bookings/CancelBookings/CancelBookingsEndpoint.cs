using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/bookings/{id:guid}/cancel", Handle).WithTags("Bookings").WithName("CancelBooking");

    private static IResult Handle(Guid id, BookingService service)
    {
        service.Cancel(id);
        return Results.NoContent();
    }
}