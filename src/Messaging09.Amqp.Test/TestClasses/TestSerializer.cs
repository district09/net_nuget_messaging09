using Apache.NMS;
using Messaging09.Amqp.Serializers;
using Messaging09.Amqp.Test.Message;

namespace Messaging09.Amqp.Test.TestClasses;

public class TestSerializer : IMessageSerializer<string>
{
    public string Deserialize(IMessage message)
    {
        return ((ITextMessage)message).Text;
    }

    public IMessage Serialize(string message, ISession session)
    {
        var x = new TestMessageFactory();
        return x.CreateTextMessage(message);
    }
}
