using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public sealed record Request(string? Name, int Capacity);

    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/rooms", Handle).WithTags("Rooms").WithName("CreateRoom");

    private static IResult Handle(Request request, RoomService service)
    {
        var room = service.Create(request.Name, request.Capacity);
        return Results.Created($"/api/rooms/{room.Id}", new Response(room.Id, room.Name, room.Capacity));
    }
}