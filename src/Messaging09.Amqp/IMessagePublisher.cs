using Apache.NMS;

namespace Messaging09.Amqp;

public interface IMessagePublisher<in TMessageType>
{
    Task SendMessage(TMessageType message, string fqdn, Action<IMessage>? messageTransform = null);
}