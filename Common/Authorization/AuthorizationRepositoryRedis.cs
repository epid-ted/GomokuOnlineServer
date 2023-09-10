using StackExchange.Redis;

namespace Common.Authorization
{
    public class AuthorizationRepositoryRedis : IAuthorizationRepository
    {
        private readonly IDatabase database;

        public AuthorizationRepositoryRedis(IDatabase database)
        {
            this.database = database;
        }

        public async Task<bool> FindUser(int userId, string sessionId)
        {
            string sessionKey = $"session:{userId}";
            return await database.StringGetAsync(sessionKey) == sessionId;
        }

        public async Task<bool> FindServer(string serverName, string sessionId)
        {
            string serverKey = $"serversession:{serverName}";
            return await database.StringGetAsync(serverKey) == sessionId;
        }
    }
}
