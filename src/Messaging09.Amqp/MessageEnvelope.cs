using Apache.NMS;

namespace Messaging09.Amqp;

public class MessageEnvelope<TMessageType>
{
    public MessageEnvelope(IMessage message, TMessageType parsed)
    {
        Original = message;
        Message = parsed;
    }

    public IMessage Original { get; init; }
    public TMessageType Message { get; init; }
}