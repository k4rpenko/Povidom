using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
namespace RedisDAL
{
    public class RedisConfigure
    {
        private readonly IConfiguration _configuration;
        public ConnectionMultiplexer redis;
        private IDatabase db;
        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(30);
        private static readonly int MaxRequests = 5;

        public RedisConfigure(IConfiguration configuration)
        {
            _configuration = configuration;
            Connect();
        }


        public void Connect()
        {
            string connectionString = _configuration.GetSection("Redis:ConnectionString").Value;
            try
            {
                redis = ConnectionMultiplexer.Connect(connectionString);
                db = redis.GetDatabase();
            }
            catch (RedisConnectionException ex)
            {
                throw new Exception("Не вдалося підключитися до Redis", ex);
            }
        }

        public bool AuthRedisUser(string ip)
        {
            var requestKey = $"requests:{ip}";

            if (!db.KeyExists(requestKey))
            {
                db.HashSet(requestKey, new HashEntry[]
                {
                    new HashEntry("Request", 1),
                    new HashEntry("Blocked", "0")
                });
                return true;
            }
            else
            {
                if (db.HashGet(requestKey, "Blocked") == "0")
                {

                    if ((int)db.HashGet(requestKey, "Request") >= MaxRequests)
                    {
                        db.HashSet(requestKey, "Blocked", DateTime.UtcNow.Add(BlockDuration).ToString());
                        return false;
                    }
                    else
                    {
                        db.HashIncrement(requestKey, "Request", 1);
                        return true;
                    }
                }
                else
                {
                    var blockedUntil = DateTime.Parse(db.HashGet(requestKey, "Blocked"));
                    if (DateTime.UtcNow < blockedUntil)
                    {
                        return false;
                    }
                    else
                    {
                        db.KeyDelete(requestKey);
                        return true;
                    }
                }
            }
        }
    }
}
