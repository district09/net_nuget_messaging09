using Messaging09.Amqp.Tracing;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

public class LogTracer : ITrace
{
    private readonly ILogger _logger;
    private readonly LogLevel _level;

    public LogTracer(ILogger logger, LogLevel level)
    {
        _logger = logger;
        _level = level;
    }

    public void Debug(string message)
    {
        if (IsDebugEnabled)
        {
            _logger.LogDebug("{Message}", message);
        }
    }

    public void Info(string message)
    {
        if (IsInfoEnabled)
        {
            _logger.LogInformation("{Message}", message);
        }
    }

    public void Warn(string message)
    {
        if (IsWarnEnabled)
        {
            _logger.LogWarning("{Message}", message);
        }
    }

    public void Error(string message)
    {
        if (IsErrorEnabled)
        {
            _logger.LogError("{Message}", message);
        }
    }

    public void Fatal(string message)
    {
        if (IsFatalEnabled)
        {
            _logger.LogCritical("{Message}", message);
        }
    }

    public bool IsDebugEnabled => _level >= LogLevel.Debug;
    public bool IsInfoEnabled => _level >= LogLevel.Info;
    public bool IsWarnEnabled => _level >= LogLevel.Warn;
    public bool IsErrorEnabled => _level >= LogLevel.Error;
    public bool IsFatalEnabled => _level >= LogLevel.Fatal;
}