using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;

namespace MatchServer.Web.Services
{
    public class MatchService
    {
        private readonly IMatchRepository matchRepository;
        private readonly IRankingRepository rankingRepository;

        public MatchService(IMatchRepository matchRepository, IRankingRepository rankingRepository)
        {
            this.matchRepository = matchRepository;
            this.rankingRepository = rankingRepository;
        }

        public async Task SaveMatchResult(MatchResultModel matchResultModel)
        {
            await matchRepository.Add(matchResultModel);

            int result = matchResultModel.Result;
            if (result > 0)
            {
                int winnerUserId = matchResultModel.Participants[result - 1];
                string winnerUsername = await rankingRepository.GetUsername(winnerUserId);
                await rankingRepository.AddScore(winnerUsername, 1);
            }
        }
    }
}
