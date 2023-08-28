using Apache.NMS;
using Messaging09.Amqp.Serializers;

namespace Messaging09.Amqp;

public abstract class MessageHandler<TMessageType>
{
    private readonly IMessageSerializer<TMessageType> _serializer;

    protected MessageHandler(IMessageSerializer<TMessageType> serializer)
    {
        _serializer = serializer;
    }

    public async Task<MessageOutcome> Handle(IMessage message)
    {
        var deserialized = _serializer.Deserialize(message);
        var envelope = new MessageEnvelope<TMessageType>(message, deserialized);
        return await Handle(envelope);
    }

    protected abstract Task<MessageOutcome> Handle(MessageEnvelope<TMessageType> envelope);
}
