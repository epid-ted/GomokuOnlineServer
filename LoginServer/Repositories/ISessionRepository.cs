namespace LoginServer.Repositories
{
    public interface ISessionRepository
    {
        Task Add(int userId, string username, string sessionId);
        Task Remove(int userId);
        Task<string?> Find(int userId);
        Task<bool> Exists(int userId);
    }
}
