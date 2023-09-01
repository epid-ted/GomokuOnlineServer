using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public interface IRankingRepository
    {
        Task<string> GetUsername(int userId);
        Task<int> GetHigherScoreCount(string username);
        Task<RankingEntry[]> GetRankings(int from, int to);
        Task AddScore(string username, int delta);
    }
}
