using StackExchange.Redis;


namespace RedisDAL.User
{
    public class RUsersConnectMessage
    {
        private readonly IDatabase _db;
        private readonly ISubscriber _subscriber;
        public Action<string> UserRemoved;
        
        public RUsersConnectMessage(RedisConfigure redisConfigure)
        {
            _db = redisConfigure.GetDatabase();
        }

        public async Task<string> GetUserId(string ConnectionId)
        {
            var key = 
        }

        public async Task UpdateUserConnection(string userId, string connectionId)
        {
            var userKEY = $"User:{userId}";

            await _db.SetAddAsync("UserConnect:Set", userId);

            await _db.HashSetAsync(userKEY, new HashEntry[]
            {
                new HashEntry("connectionId", connectionId),
                new HashEntry("status", true),
                new HashEntry("lastPingTime", DateTime.UtcNow.ToString())
            });
            await _db.KeyExpireAsync(userKEY, TimeSpan.FromMinutes(3));

        }

        public async Task<string> GetUserConnectionId(string userId)
        {

        }

        public async Task RemoveUser(string userId)
        {

        }


        public async Task<IEnumerable<string>> GetAllUsers()
        {
            
        }
    }
}
