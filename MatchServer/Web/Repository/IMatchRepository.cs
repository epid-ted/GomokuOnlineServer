using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public interface IMatchRepository
    {
        Task Add(MatchResultModel matchResultModel);
    }
}
