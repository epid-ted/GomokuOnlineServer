using StackExchange.Redis;

namespace GameServer.Configuration
{
    public class RedisConfig
    {
        public static IConnectionMultiplexer Redis { get; set; }
    }
}
