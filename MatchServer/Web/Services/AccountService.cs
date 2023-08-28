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

        public async Task UpdateStamina(int userId)
        {
            await accountRepository.Update(userId);
        }
    }
}
