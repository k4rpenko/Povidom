namespace UserServer.Protection
{
    public class RateLimitingMiddleware
    {
        private static readonly Dictionary<string, (int RequestCount, DateTime FirstRequestTime)> _requestCounts = new();
        private const int MAX_REQUESTS = 100;
        private static readonly TimeSpan TIME_WINDOW = TimeSpan.FromMinutes(1);

        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();

            lock (_requestCounts) 
            {
                if (!_requestCounts.TryGetValue(ip, out var requestInfo))
                {
                    requestInfo = (0, DateTime.UtcNow); 
                }

                if (DateTime.UtcNow - requestInfo.FirstRequestTime > TIME_WINDOW)
                {
                    requestInfo = (1, DateTime.UtcNow);
                }
                else
                {
                    requestInfo.RequestCount++; 
                }

                if (requestInfo.RequestCount > MAX_REQUESTS) 
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests; 
                    return; 
                }

                _requestCounts[ip] = requestInfo; 
            }

            await _next(context);
        }
    }

}
