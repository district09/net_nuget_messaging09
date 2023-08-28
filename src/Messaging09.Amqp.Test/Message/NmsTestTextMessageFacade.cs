using Apache.NMS.AMQP.Message;
using Apache.NMS.AMQP.Message.Facade;
using Apache.NMS.AMQP.Provider.Amqp;

namespace Messaging09.Amqp.Test.Message;

public class NmsTestTextMessageFacade : NmsTestMessageFacade, INmsTextMessageFacade
{
    public void Initialize(AmqpConsumer consumer)
    {
    }

    public override NmsMessage AsMessage()
    {
        return new NmsTextMessage(this);
    }

    public override void ClearBody()
    {
        Text = null;
    }

    public string Text { get; set; }
}
