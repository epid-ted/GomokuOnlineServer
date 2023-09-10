namespace Common.Authorization
{
    public interface IAuthorizationRepository
    {
        Task<bool> FindUser(int userId, string sessionId);
        Task<bool> FindServer(string serverName, string sessionId);
    }
}
