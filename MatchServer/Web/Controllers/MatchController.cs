using Common.Authorization;
using MatchServer.Web.Data.DTOs.GameServer;
using MatchServer.Web.Data.Models;
using MatchServer.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("match")]
    public class MatchController : ControllerBase
    {
        private readonly MatchService matchService;
        private readonly RankingService rankingService;
        private readonly StaminaService staminaService;
        private readonly AuthorizationService authorizationService;

        public MatchController(AuthorizationService authorizationService, MatchService matchService, RankingService rankingService, StaminaService staminaService)
        {
            this.authorizationService = authorizationService;
            this.matchService = matchService;
            this.rankingService = rankingService;
            this.staminaService = staminaService;
        }

        [HttpPost("result")]
        public async Task<IActionResult> SaveMatchResult(SaveMatchResultRequestDto dto, [FromHeader] string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var scheme = headerValue.Scheme;
                var serverSessionId = headerValue.Parameter;
                if (scheme != "ServerSessionId" || serverSessionId == null || !await authorizationService.AuthorizeHttpRequestFromServer("GameServer", serverSessionId))
                {
                    return BadRequest();
                }
            }

            MatchResultModel matchResultModel = new MatchResultModel()
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Result = dto.Result,
                UserIds = dto.UserIds,
                Usernames = dto.Usernames
            };
            await matchService.SaveMatchResult(matchResultModel);

            int result = dto.Result;
            if (result > 0)
            {
                string winnerUsername = dto.Usernames[result - 1];
                await rankingService.UpdateRanking(winnerUsername);
            }
            else if (result == -1)
            {
                // Restore stamina when the game is invalid
                for (int i = 0; i < matchResultModel.UserIds.Length; i++)
                {
                    await staminaService.AddStamina(dto.UserIds[i], 10);
                }
            }
            return Ok();
        }
    }
}