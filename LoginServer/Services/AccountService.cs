using LoginServer.Data.Models;
using LoginServer.Repositories;

namespace LoginServer.Services
{
    public class AccountService
    {
        private readonly IAccountRepository accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public async Task Register(string username, string password)
        {
            string encodedPassword = EncodePassword(password);
            await accountRepository.Add(username, encodedPassword);
            return;
        }

        public async Task<UserModel?> Authenticate(string username, string password)
        {
            UserModel? userModel = await accountRepository.Find(username);
            if (userModel == null)
            {
                return null;
            }

            bool verified = VerifyPassword(password, userModel.EncodedPassword);
            if (verified)
            {
                return userModel;
            }
            else
            {
                return null;
            }
        }

        private string EncodePassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        private bool VerifyPassword(string password, string encodedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, encodedPassword);
        }
    }
}
