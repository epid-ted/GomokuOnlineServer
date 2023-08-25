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

        public async Task Add(int userId, string sessionId)
        {
            string redisKey = MakeRedisSessionKey(userId);
            await database.StringSetAsync(redisKey, sessionId, new TimeSpan(1, 0, 0));
        }

        public async Task Remove(int userId)
        {
            string redisKey = MakeRedisSessionKey(userId);
            await database.KeyDeleteAsync(redisKey);
        }

        public async Task<string?> Find(int userId)
        {
            string redisKey = MakeRedisSessionKey(userId);
            return await database.StringGetAsync(redisKey);
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
