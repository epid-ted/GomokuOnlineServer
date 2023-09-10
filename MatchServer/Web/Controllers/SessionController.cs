using Common.Authorization;
using MatchServer.WaitingQueue;
using Microsoft.AspNetCore.Mvc;
using Server.Session;
using System.Net.Http.Headers;

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
