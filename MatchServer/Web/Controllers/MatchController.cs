using MatchServer.Web.Data.DTOs.Client;
using MatchServer.Web.Data.DTOs.GameServer;
using MatchServer.Web.Data.Models;
using MatchServer.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("match")]
    public class MatchController : ControllerBase
    {
        private readonly MatchService matchService;
        private readonly RankingService rankingService;
        private readonly StaminaService accountService;

        public MatchController(MatchService matchService, RankingService rankingService, StaminaService accountService)
        {
            this.matchService = matchService;
            this.rankingService = rankingService;
            this.accountService = accountService;
        }

        [HttpGet("stamina/{userId}")]
        public async Task<IActionResult> GetStamina(int userId)
        {
            StaminaModel staminaModel = await accountService.GetStamina(userId);
            if (staminaModel.Stamina == -1)
            {
                return BadRequest();
            }
            GetStaminaResponseDto getStaminaResponseDto = new GetStaminaResponseDto()
            {
                LastStaminaUpdateTime = staminaModel.LastStaminaUpdateTime,
                Stamina = staminaModel.Stamina
            };
            return Ok(getStaminaResponseDto);
        }

        [HttpPost("result/save")]
        public async Task<IActionResult> SaveMatchResult(SaveMatchResultRequestDto saveMatchResultDto)
        {
            MatchResultModel matchResultModel = new MatchResultModel()
            {
                StartTime = saveMatchResultDto.StartTime,
                EndTime = saveMatchResultDto.EndTime,
                Result = saveMatchResultDto.Result,
                UserIds = saveMatchResultDto.UserIds,
                Usernames = saveMatchResultDto.Usernames
            };
            await matchService.SaveMatchResult(matchResultModel);

            int result = saveMatchResultDto.Result;
            if (result > 0)
            {
                string winnerUsername = saveMatchResultDto.Usernames[result - 1];
                await rankingService.UpdateRanking(winnerUsername);
            }
            else if (result == -1)
            {
                // Restore stamina when the game is invalid
                for (int i = 0; i < matchResultModel.UserIds.Length; i++)
                {
                    await accountService.AddStamina(saveMatchResultDto.UserIds[i], 10);
                }
            }
            return Ok();
        }
    }
}