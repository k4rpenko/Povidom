using Microsoft.Extensions.Configuration;
using RedisDAL.Interface;
using StackExchange.Redis;
namespace RedisDAL
{
    public class RedisConfigure : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly IConfiguration _configuration;
        public ConnectionMultiplexer redis;

        public RedisConfigure(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetSection("Redis:ConnectionString").Value;

            try
            {
                _redis = ConnectionMultiplexer.Connect(connectionString);
                _db = _redis.GetDatabase();
            }
            catch (RedisConnectionException ex)
            {
                throw new Exception("Не вдалося підключитися до Redis", ex);
            }
        }

        public IDatabase GetDatabase()
        {
            return _db;
        }

        public ISubscriber GetSubscriber() { 
            return _redis.GetSubscriber(); 
        }
    }
}
