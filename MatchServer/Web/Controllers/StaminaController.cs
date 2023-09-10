using Common.Authorization;
using MatchServer.Web.Data.DTOs.Client;
using MatchServer.Web.Data.Models;
using MatchServer.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("stamina")]
    public class StaminaController : ControllerBase
    {
        private readonly AuthorizationService authorizationService;
        private readonly StaminaService staminaService;

        public StaminaController(AuthorizationService authorizationService, StaminaService staminaService)
        {
            this.authorizationService = authorizationService;
            this.staminaService = staminaService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetStamina(int userId, [FromHeader] string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var sessionId = headerValue.Parameter;
                if (scheme != "SessionId" || sessionId == null || !await authorizationService.AuthorizeHttpRequestFromUser(userId, sessionId))
                {
                    return BadRequest();
                }
            }

            StaminaModel staminaModel = await staminaService.GetStamina(userId);
            if (staminaModel.Stamina == -1)
            {
                return BadRequest();
            }
            GetStaminaResponseDto getStaminaResponseDto = new GetStaminaResponseDto()
            {
                LastUpdated = staminaModel.LastUpdated,
                Stamina = staminaModel.Stamina
            };
            return Ok(getStaminaResponseDto);
        }
    }
}
