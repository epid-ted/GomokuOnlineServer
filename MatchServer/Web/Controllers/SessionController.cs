using Common.Authorization;
using MatchServer.WaitingQueue;
using MatchServer.Web.Data.DTOs.LoginServer;
using Microsoft.AspNetCore.Mvc;
using Server.Session;
using System.Diagnostics;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("session")]
    public class SessionController : ControllerBase
    {
        private readonly AuthorizationService authorizationService;

        public SessionController(AuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpPost("kickout")]
        public async Task<IActionResult> Kickout(KickoutRequestDto dto)
        {
            int userId = dto.UserId;
            Debug.WriteLine($"Kickout UserId:{userId}");

            if (dto.ServerName != "LoginServer" || !await authorizationService.AuthorizeHttpRequestFromServer(dto.ServerName, dto.ServerSessionId))
            {
                return BadRequest();
            }

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
