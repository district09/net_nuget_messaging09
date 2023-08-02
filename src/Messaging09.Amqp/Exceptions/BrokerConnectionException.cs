using System.Runtime.Serialization;

namespace Messaging09.Amqp.Exceptions;

[Serializable]
public class BrokerConnectionException : Exception
{
    public override string Message => $"Could not connect to {Host} in {TimeoutSeconds} seconds";
    public string? Host { get; }
    public int TimeoutSeconds { get; }

    protected BrokerConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Host = info.GetString("host");
        TimeoutSeconds = info.GetInt32("timeoutSeconds");
    }

    public BrokerConnectionException(string host, int seconds)
    {
        Host = host;
        TimeoutSeconds = seconds;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        info.AddValue("host", Host);
        info.AddValue("timeoutSeconds", TimeoutSeconds);
        base.GetObjectData(info, context);
    }
}