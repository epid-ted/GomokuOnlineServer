using LoginServer.Data.Models;

namespace LoginServer.Repositories
{
    public interface IAccountRepository
    {
        Task Add(string username, string encodedPassword);
        Task<UserModel?> Find(string username);
    }
}
