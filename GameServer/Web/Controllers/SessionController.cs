using Common.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Session;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace GameServer.Web.Controllers
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

        [HttpPost("kickout/{userId}")]
        public async Task<IActionResult> KickoutAsync([FromRoute] int userId, [FromHeader] string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var serverSessionId = headerValue.Parameter;
                if (scheme != "ServerSessionId" || serverSessionId == null || !await authorizationService.AuthorizeHttpRequestFromServer("LoginServer", serverSessionId))
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

            Debug.WriteLine($"Kickout UserId:{userId}");

            ClientSession? session = SessionManager.Instance.Find(userId);
            if (session != null)
            {
                SessionManager.Instance.Remove(userId);
                session.Disconnect();
                session.Room = null;
            }
            return Ok();
        }
    }
}