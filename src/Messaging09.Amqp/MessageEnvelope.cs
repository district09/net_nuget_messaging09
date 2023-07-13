using Apache.NMS;

namespace Messaging09.Amqp
{
    public class MessageEnvelope<TMessageType>
    {
        public MessageEnvelope(IMessage message, TMessageType parsed)
        {
            Original = message;
            Message = parsed;
        }

        public IMessage Original { get; set; }
        public TMessageType Message { get; set; }
    }
}