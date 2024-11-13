using RedisDAL;

namespace Server.Protection
{
    public class IpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RedisConfigure _redisConfig;

        public IpLoggingMiddleware(RequestDelegate next, RedisConfigure redisConfig)
        {
            _next = next;
            _redisConfig = redisConfig;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/Auth/login"))
            {
                var ipAddress = context.Connection.RemoteIpAddress.ToString();

                if (!_redisConfig.AuthRedisUser(ipAddress))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. IP blocked.");
                    return;
                }
            }
            await _next(context);
        }
    }
}
