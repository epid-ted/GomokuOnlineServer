using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;

namespace MatchServer.Web.Services
{
    public class MatchService
    {
        private readonly IMatchRepository matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public async Task SaveMatchResult(MatchResultModel matchResultModel)
        {
            await matchRepository.Add(matchResultModel);
        }
    }
}
