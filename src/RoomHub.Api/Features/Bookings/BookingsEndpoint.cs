using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;

namespace RoomHub.Api.Features.Bookings;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookings(this IEndpointRouteBuilder app)
    {
        CreateBookingEndpoint.Map(app);
        GetBookingEndpoint.Map(app);
        CancelBookingEndpoint.Map(app);
        return app;
    }
}