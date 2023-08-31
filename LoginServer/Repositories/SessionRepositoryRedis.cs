using StackExchange.Redis;

namespace LoginServer.Repositories
{
    public class SessionRepositoryRedis : ISessionRepository
    {
        private readonly IDatabase database;

        public SessionRepositoryRedis(IDatabase database)
        {
            this.database = database;
        }

        public async Task Add(int userId, string username, string sessionId)
        {
            string sessionKey = $"session:{userId}";
            await database.StringSetAsync(sessionKey, sessionId, new TimeSpan(1, 0, 0));

            string usernameKey = $"username:{userId}";
            await database.StringSetAsync(usernameKey, username, new TimeSpan(1, 0, 0));
        }

        public async Task Remove(int userId)
        {
            string sessionKey = $"session:{userId}";
            await database.KeyDeleteAsync(sessionKey);

            string usernameKey = $"username:{userId}";
            await database.KeyDeleteAsync(usernameKey);
        }

        public async Task<string?> Find(int userId)
        {
            string sessionKey = $"session:{userId}";
            return await database.StringGetAsync(sessionKey);
        }

        public async Task<bool> Exists(int userId)
        {
            string redisKey = MakeRedisSessionKey(userId);
            return await database.KeyExistsAsync(redisKey);
        }

        private string MakeRedisSessionKey(int userId)
        {
            return "session:" + userId.ToString();
        }
    }
}
