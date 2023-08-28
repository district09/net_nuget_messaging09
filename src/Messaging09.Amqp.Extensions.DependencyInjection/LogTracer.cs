using Messaging09.Amqp.Tracing;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class LogTracer : ITrace
{
    private readonly ILogger _logger;

    public LogTracer(ILogger logger)
    {
        _logger = logger;
    }

    public void Debug(string message)
    {
        _logger.LogDebug("{Message}", message);
    }

    public void Info(string message)
    {
        _logger.LogInformation("{Message}", message);
    }

    public void Warn(string message)
    {
        _logger.LogWarning("{Message}", message);
    }

    public void Error(string message)
    {
        _logger.LogError("{Message}", message);
    }

    public void Fatal(string message)
    {
        _logger.LogCritical("{Message}", message);
    }

    public bool IsDebugEnabled => true;
    public bool IsInfoEnabled => true;
    public bool IsWarnEnabled => true;
    public bool IsErrorEnabled => true;
    public bool IsFatalEnabled => true;
}
