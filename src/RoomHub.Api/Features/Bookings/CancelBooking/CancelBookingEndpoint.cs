using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/bookings/{id:guid}/cancel", Handle).WithTags("Bookings").WithName("CancelBooking");

    private static async Task<Results<NoContent, ProblemHttpResult>> Handle(
        Guid id,
        ICommandHandler<CancelBookingCommand, Unit> handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(new CancelBookingCommand(id), ct);
        if (result.IsFailure)
            return result.Error.ToProblem();

        return TypedResults.NoContent();
    }
}
