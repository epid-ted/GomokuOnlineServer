using MatchServer.Web.Data.Models;
using StackExchange.Redis;

namespace MatchServer.Web.Repository
{
    public class RankingRepositoryRedis : IRankingRepository
    {
        private readonly IDatabase database;
        private readonly string rankingKey = "ranking";

        public RankingRepositoryRedis(IDatabase database)
        {
            this.database = database;
        }

        public async Task<string> GetUsername(int userId)
        {
            string usernameKey = $"username:{userId}";
            return await database.StringGetAsync(usernameKey);
        }

        // Returns -1 if username not found
        public async Task<int> GetHigherScoreCount(string username)
        {
            double? score = await database.SortedSetScoreAsync(rankingKey, username);

            if (score == null)
            {
                return -1;
            }
            else
            {
                return (int)await database.SortedSetLengthByValueAsync(rankingKey, score, double.PositiveInfinity);
            }
        }

        public async Task<RankingEntry[]> GetRankings(int from, int to)
        {
            SortedSetEntry[] sortedSetEntries = await database.SortedSetRangeByRankWithScoresAsync(rankingKey, from, to, order: Order.Descending);

            int i = from;
            return Array.ConvertAll(sortedSetEntries, x => new RankingEntry
            {
                Rank = i++,
                Username = x.Element,
                WinCount = (int)x.Score
            });
        }

        public async Task AddScore(string username, int delta)
        {
            await database.SortedSetIncrementAsync(rankingKey, username, delta);
        }
    }
}
