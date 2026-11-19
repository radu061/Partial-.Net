using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public sealed record Response(
        Guid Id, Guid RoomId, string BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees, BookingStatus Status);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/bookings/{id:guid}", Handle).WithTags("Bookings").WithName("GetBooking");

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Guid id, IBookingRepository bookings, CancellationToken ct)
    {
        var b = await bookings.GetByIdAsync(id, ct);
        if (b is null)
            return BookingErrors.NotFound.ToProblem();

        return TypedResults.Ok(new Response(
            b.Id, b.RoomId, b.BookedBy, b.Range.StartUtc, b.Range.EndUtc, b.Attendees, b.Status));
    }
}
