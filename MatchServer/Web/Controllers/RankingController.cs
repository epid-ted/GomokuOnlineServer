using Common.Authorization;
using MatchServer.Web.Data.Models;
using MatchServer.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Web.Controllers
{
    [ApiController]
    [Route("ranking")]
    public class RankingController : ControllerBase
    {
        private readonly RankingService rankingService;
        private readonly AuthorizationService authorizationService;

        public RankingController(RankingService rankingService, AuthorizationService authorizationService)
        {
            this.rankingService = rankingService;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking(int userId, string sessionId)
        {
            if (!await authorizationService.AuthorizeHttpRequestFromUser(userId, sessionId))
            {
                return BadRequest();
            }
            int ranking = await rankingService.GetRanking(userId);
            return Ok(ranking);
        }

        [HttpGet]
        public async Task<IActionResult> GetRankings([FromQuery] int from, [FromQuery] int to)
        {
            int length = to - from + 1;
            if (from < 0 || length < 0 || length > 100)
            {
                return BadRequest();
            }

            RankingEntry[] rankingEntries = await rankingService.GetRankings(from, to);
            return Ok(rankingEntries);
        }
    }
}
