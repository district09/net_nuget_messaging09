using Apache.NMS;

namespace Messaging09.Amqp;

public interface IMessagePublisher
{
    Task SendMessage<TMessageType>(TMessageType message, string queue, Action<IMessage>? messageTransform = null);
}
