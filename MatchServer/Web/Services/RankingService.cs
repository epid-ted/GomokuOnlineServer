using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;

namespace MatchServer.Web.Services
{
    public class RankingService
    {
        private readonly IRankingRepository rankingRepository;

        public RankingService(IRankingRepository rankingRepository)
        {
            this.rankingRepository = rankingRepository;
        }

        public async Task<string> GetUsername(int userId)
        {
            return await rankingRepository.GetUsername(userId);
        }

        public async Task<int> GetRanking(string username)
        {
            int higherScoreCount = await rankingRepository.GetHigherScoreCount(username);
            int ranking = higherScoreCount + 1;
            return ranking;
        }

        public async Task<RankingEntry[]> GetRankings(int from, int to)
        {
            RankingEntry[] rankingEntries = await rankingRepository.GetRankings(from, to);

            // Users with tied win count are assigned the same rank
            if (rankingEntries.Length > 0)
            {
                rankingEntries[0].Rank = 1;
                int sameScore = 1;

                for (int i = 1; i < rankingEntries.Length; i++)
                {
                    if (rankingEntries[i].WinCount == rankingEntries[i - 1].WinCount)
                    {
                        rankingEntries[i].Rank = rankingEntries[i - 1].Rank;
                        sameScore++;
                    }
                    else
                    {
                        rankingEntries[i].Rank = rankingEntries[i - 1].Rank + sameScore;
                        sameScore = 1;
                    }
                }
            }
            return rankingEntries;
        }
    }
}
