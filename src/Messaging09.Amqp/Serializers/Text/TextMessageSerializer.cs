using Apache.NMS;
using MessageFormatException = Messaging09.Amqp.Exceptions.MessageFormatException;

namespace Messaging09.Amqp.Serializers.Text;

public abstract class TextMessageSerializer<TMessageType> : IMessageSerializer<TMessageType>
{
    public TMessageType Deserialize(IMessage message)
    {
        if (message is not ITextMessage textMessage)
        {
            throw new MessageFormatException(typeof(TextMessageSerializer<TMessageType>), typeof(ITextMessage),
                message.GetType());
        }

        return DeserializeFromText(textMessage.Text);
    }

    public IMessage Serialize(TMessageType message, ISession session)
    {
        var serialized = SerializeToText(message);
        var textMessage = session.CreateTextMessage(serialized);
        return textMessage;
    }

    protected abstract TMessageType DeserializeFromText(string text);
    protected abstract string SerializeToText(TMessageType message);
}