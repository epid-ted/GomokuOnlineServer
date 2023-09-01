using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;

namespace MatchServer.Web.Services
{
    public class StaminaService
    {
        private readonly IStaminaRepository accountRepository;

        public StaminaService(IStaminaRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<StaminaModel> GetStamina(int userId)
        {
            return await accountRepository.Get(userId);
        }

        public async Task AddStamina(int userId, int value)
        {
            await accountRepository.AddStamina(userId, value);
        }
    }
}
