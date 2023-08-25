using MatchServer.Web.Data.DTOs.GameServer;
using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("match")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository matchRepository;

        public MatchController(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
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
            await matchRepository.Add(matchResultModel);
            return Ok();
        }
    }
}