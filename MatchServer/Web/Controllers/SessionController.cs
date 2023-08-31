using MatchServer.WaitingQueue;
using Microsoft.AspNetCore.Mvc;
using Server.Session;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("session")]
    public class SessionController : ControllerBase
    {
        [HttpPost("kickout")]
        public IActionResult Kickout([FromQuery] int userId)
        {
            Console.WriteLine($"Kickout UserId:{userId}");

            ClientSession? session = SessionManager.Instance.Find(userId);
            if (session != null)
            {
                SessionManager.Instance.Remove(userId);
                session.Disconnect();
            }

            UserQueue.Instance.Remove(userId);
            return Ok();
        }
    }
}
