#pragma warning disable CS8604
namespace Messaging09.Amqp.Tracing;

public static class Tracer
{
    // prevent instantiation of this class. All methods are static.

    public static ITrace? Trace { get; set; }

    private static bool IsDebugEnabled => Trace is { IsDebugEnabled: true };

    private static bool IsInfoEnabled => Trace is { IsInfoEnabled: true };

    private static bool IsWarnEnabled => Trace is { IsWarnEnabled: true };

    private static bool IsErrorEnabled => Trace is { IsErrorEnabled: true };

    private static bool IsFatalEnabled => Trace is { IsFatalEnabled: true };

    public static void Debug(object message)
    {
        if (IsDebugEnabled)
        {
            Trace!.Debug(message.ToString());
        }
    }

    public static void DebugFormat(string format, params object[] args)
    {
        if (IsDebugEnabled)
        {
            Trace!.Debug(string.Format(format, args));
        }
    }

    public static void Info(object message)
    {
        if (IsInfoEnabled)
        {
            Trace!.Info(message.ToString());
        }
    }

    public static void InfoFormat(string format, params object[] args)
    {
        if (IsInfoEnabled)
        {
            Trace!.Info(string.Format(format, args));
        }
    }

    public static void Warn(object message)
    {
        if (IsWarnEnabled)
        {
            Trace!.Warn(message.ToString());
        }
    }

    public static void WarnFormat(string format, params object[] args)
    {
        if (IsWarnEnabled)
        {
            Trace!.Warn(string.Format(format, args));
        }
    }

    public static void Error(object message)
    {
        if (IsErrorEnabled)
        {
            Trace!.Error(message.ToString());
        }
    }

    public static void ErrorFormat(string format, params object[] args)
    {
        if (IsErrorEnabled)
        {
            Trace!.Error(string.Format(format, args));
        }
    }

    public static void Fatal(object message)
    {
        if (IsFatalEnabled)
        {
            Trace!.Fatal(message.ToString());
        }
    }

    public static void FatalFormat(string format, params object[] args)
    {
        if (IsFatalEnabled)
        {
            Trace!.Fatal(string.Format(format, args));
        }
    }
}