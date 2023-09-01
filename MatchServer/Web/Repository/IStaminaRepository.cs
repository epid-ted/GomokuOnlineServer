using MatchServer.Web.Data.Models;

namespace MatchServer.Web.Repository
{
    public interface IStaminaRepository
    {
        Task<StaminaModel> Get(int userId);
        Task AddStamina(int userId, int value);
    }
}
