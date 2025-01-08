using RedisDAL.Interface;
using StackExchange.Redis;
using System;

namespace RedisDAL.User
{
    public class RAuthUser
    {
        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(30);
        private static readonly int MaxRequests = 5;
        private readonly IDatabase _db;

        public RAuthUser(RedisConfigure redisConfigure)
        {
            _db = redisConfigure.GetDatabase();
        }

        public bool AuthRedisUser(string ip)
        {
            var requestKey = $"requests:{ip}";
            if (!_db.KeyExists(requestKey))
            {
                _db.HashSet(requestKey, new HashEntry[]
                {
                    new HashEntry("Request", 1),
                    new HashEntry("Blocked", "0")
                });
                _db.KeyExpire(requestKey, TimeSpan.FromMinutes(30));
                return true;
            }
            else
            {
                if (_db.HashGet(requestKey, "Blocked") == "0")
                {
                    if ((int)_db.HashGet(requestKey, "Request") >= MaxRequests)
                    {
                        _db.HashSet(requestKey, "Blocked", DateTime.UtcNow.Add(BlockDuration).ToString());
                        return false;
                    }
                    else
                    {
                        _db.HashIncrement(requestKey, "Request", 1);
                        return true;
                    }
                }
                else
                {
                    var blockedUntil = DateTime.Parse(_db.HashGet(requestKey, "Blocked"));
                    if (DateTime.UtcNow < blockedUntil)
                    {
                        return false;
                    }
                    else
                    {
                        _db.KeyDelete(requestKey);
                        return true;
                    }
                }
            }
        }
    }
}
