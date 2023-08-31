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

        public RankingController(RankingService rankingService)
        {
            this.rankingService = rankingService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRanking([FromRoute] int userId)
        {
            string username = await rankingService.GetUsername(userId);
            int ranking = await rankingService.GetRanking(username);
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
