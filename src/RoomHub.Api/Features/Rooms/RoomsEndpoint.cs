using RoomHub.Api.Features.Rooms.CreateRoom;
using RoomHub.Api.Features.Rooms.GetRoom;

namespace RoomHub.Api.Features.Rooms;

public static class RoomsEndpoints
{
    public static IEndpointRouteBuilder MapRooms(this IEndpointRouteBuilder app)
    {
        CreateRoomEndpoint.Map(app);
        GetRoomEndpoint.Map(app);
        return app;
    }
}