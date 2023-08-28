using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public interface IAccountRepository
    {
        public Task<StaminaModel> Get(int userId);
        public Task Update(int userId);
    }
}
