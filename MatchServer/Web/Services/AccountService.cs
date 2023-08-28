using MatchServer.Web.Data.Models;
using MatchServer.Web.Repository;

namespace MatchServer.Web.Services
{
    public class AccountService
    {
        private readonly IAccountRepository accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task<StaminaModel> GetStamina(int userId)
        {
            return await accountRepository.Get(userId);
        }

        public async Task AddStamina(int userId, int value)
        {
            await accountRepository.Add(userId, value);
        }
    }
}
