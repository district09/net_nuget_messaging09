using Apache.NMS;

namespace Messaging09.Amqp.Serializers
{
    public interface IMessageSerializer<TMessageType>
    {
        TMessageType Deserialize(IMessage message);
        IMessage Serialize(TMessageType message, ISession session);
    }
}