using StackExchange.Redis;

namespace MatchServer.Configuration
{
    public class RedisConfig
    {
        public static IConnectionMultiplexer Redis { get; set; }
    }
}
