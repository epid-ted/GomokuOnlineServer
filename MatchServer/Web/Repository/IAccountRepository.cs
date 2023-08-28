using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public interface IAccountRepository
    {
        public Task<StaminaModel> Get(int userId);
        public Task Add(int userId, int value);
    }
}
