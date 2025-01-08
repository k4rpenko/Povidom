using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDAL.Interface
{
    public interface IRedisService
    {
        IDatabase GetDatabase();
    }
}
