using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

// A read, not a command: no handler, the endpoint reads through the repository.
public static class GetRoomEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/rooms/{id:guid}", Handle).WithTags("Rooms").WithName("GetRoom");

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Guid id, IRoomRepository rooms, CancellationToken ct)
    {
        var room = await rooms.GetByIdAsync(id, ct);
        if (room is null)
            return RoomErrors.NotFound.ToProblem();

        return TypedResults.Ok(new Response(room.Id, room.Name, room.Capacity));
    }
}
