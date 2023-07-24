using Apache.NMS;

namespace Messaging09.Amqp.Serializers;

public interface IMessageSerializer
{
}

public interface IMessageSerializer<TMessageType> : IMessageSerializer
{
    public TMessageType Deserialize(IMessage message);
    public IMessage Serialize(TMessageType message, ISession session);
}