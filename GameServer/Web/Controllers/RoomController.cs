using GameServer.Room;
using GameServer.Web.Data.DTOs.GameServer;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Controllers
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        [HttpPost("create")]
        public IActionResult CreateRoom(CreateRoomRequestDto createRoomRequestDto)
        {
            GameRoom room = RoomManager.Instance.Create();
            for (int i = 0; i < createRoomRequestDto.Participants.Length; i++)
            {
                room.PlayerIds[i] = createRoomRequestDto.Participants[i];
            }

            CreateRoomResponseDto createRoomResponseDto = new CreateRoomResponseDto()
            {
                RoomId = room.RoomId
            };
            return Ok(createRoomResponseDto);
        }
    }
}
