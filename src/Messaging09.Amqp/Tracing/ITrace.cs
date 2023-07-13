namespace Messaging09.Amqp.Tracing;

/// <summary>
/// The ITrace interface is used internally by ActiveMQ to log messages.
/// The client application may provide an implementation of ITrace if it wishes to
/// route messages to a specific destination.
/// </summary>
/// <remarks>
/// <para>
/// Use the <see cref="Apache.NMS.Tracer"/> class to register an instance of ITrace as the
/// active trace destination.
/// </para>
/// </remarks>
public interface ITrace
{
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message);
    void Fatal(string message);

    bool IsDebugEnabled { get; }
    bool IsInfoEnabled { get; }
    bool IsWarnEnabled { get; }
    bool IsErrorEnabled { get; }
    bool IsFatalEnabled { get; }
}