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
        private readonly AccountService accountService;

        public MatchController(MatchService matchService, AccountService accountService)
        {
            this.matchService = matchService;
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
                Participants = saveMatchResultDto.Participants
            };
            await matchService.SaveMatchResult(matchResultModel);
            for (int i = 0; i < matchResultModel.Participants.Length; i++)
            {
                await accountService.UpdateStamina(saveMatchResultDto.Participants[i]);
            }
            return Ok();
        }
    }
}