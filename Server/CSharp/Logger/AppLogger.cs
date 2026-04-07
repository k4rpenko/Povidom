using Microsoft.Extensions.Logging;

namespace Logger;

public class AppLogger<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;
    
    public AppLogger(ILogger<T> logger)
    {
        _logger = logger;
    }


    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(string.Format(message, args));
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(string.Format(message, args));
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, string.Format(message, args));
    }
}