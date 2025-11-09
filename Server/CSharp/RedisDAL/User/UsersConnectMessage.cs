using StackExchange.Redis;
using System;

namespace RedisDAL.User
{
    public class UsersConnectMessage
    {
        private readonly IDatabase _db;
        private readonly ISubscriber _subscriber;
        public Action<string> UserRemoved;

        public UsersConnectMessage(RedisConfigure redisConfigure)
        {
            _db = redisConfigure.GetDatabase();
            _subscriber = redisConfigure.GetSubscriber();
            _subscriber.Unsubscribe("__keyevent@0__:expired");
            _subscriber.Subscribe("__keyevent@0__:expired", async (channel, message) =>
            {
                var expiredKey = message.ToString();
                if (expiredKey.StartsWith("UsersConnect:"))
                {
                    var userId = expiredKey.Substring("UsersConnect:".Length);
                    if (!await _db.KeyExistsAsync(expiredKey))
                    {
                        UserRemoved?.Invoke(userId);
                    }
                }
            });

        }

        public async Task UpdateUserConnection(string userId, string connectionId)
        {
            var key = $"UsersConnect:{userId}";
            var fields = new HashEntry[]
            {
                new HashEntry("connectionId", connectionId),
                new HashEntry("lastPingTime", DateTime.UtcNow.ToString())
            };
            await _db.HashSetAsync(key, fields);
            await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(7));
        }

        public async Task<string> GetUserConnectionId(string userId)
        {
            var key = $"UsersConnect:{userId}";
            var connectionId = await _db.HashGetAsync(key, "connectionId");
            return connectionId.ToString();
        }


        public async Task RemoveUser(string userId)
        {
            var key = $"UsersConnect:{userId}";
            await _db.KeyDeleteAsync(key);
        }


        public async Task<DateTime?> GetLastPingTime(string userId)
        {
            var key = $"UsersConnect:{userId}";
            var lastPing = await _db.HashGetAsync(key, "lastPingTime");
            return lastPing.HasValue ? DateTime.Parse(lastPing) : null;
        }


        public async Task<IEnumerable<string>> GetAllUsers()
        {
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
            return server.Keys(pattern: "UsersConnect:*").Select(k => k.ToString());
        }
    }
}
