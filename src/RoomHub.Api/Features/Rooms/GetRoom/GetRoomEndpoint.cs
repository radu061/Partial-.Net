using RoomHub.Api.Services;
namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/rooms/{id:guid}", Handle).WithTags("Rooms").WithName("GetRoom");

    private static IResult Handle(Guid id, RoomService service)
    {
        var room = service.Get(id);
        return Results.Ok(new Response(room.Id, room.Name, room.Capacity));
    }
}