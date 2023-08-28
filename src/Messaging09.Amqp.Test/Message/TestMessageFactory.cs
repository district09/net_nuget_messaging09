using Apache.NMS.AMQP.Message;

namespace Messaging09.Amqp.Test.Message;

public class TestMessageFactory : INmsMessageFactory
{
    public NmsMessage CreateMessage()
    {
        return new NmsMessage(new NmsTestMessageFacade());
    }

    public NmsTextMessage CreateTextMessage()
    {
        return CreateTextMessage(null);
    }

    public NmsTextMessage CreateTextMessage(string payload)
    {
        return new NmsTextMessage(new NmsTestTextMessageFacade()) { Text = payload };
    }

    public NmsStreamMessage CreateStreamMessage()
    {
        return new NmsStreamMessage(new NmsTestStreamMessageFacade());
    }

    public NmsBytesMessage CreateBytesMessage()
    {
        return new NmsBytesMessage(new NmsTestBytesMessageFacade());
    }

    public NmsBytesMessage CreateBytesMessage(byte[] body)
    {
        NmsBytesMessage bytesMessage = CreateBytesMessage();
        bytesMessage.Content = body;
        return bytesMessage;
    }

    public NmsMapMessage CreateMapMessage()
    {
        return new NmsMapMessage(new NmsTestMapMessageFacade());
    }

    public NmsObjectMessage CreateObjectMessage()
    {
        return new NmsObjectMessage(new NmsTestObjectMessageFacade());
    }

    public NmsObjectMessage CreateObjectMessage(object body)
    {
        NmsObjectMessage objectMessage = CreateObjectMessage();
        objectMessage.Body = body;
        return objectMessage;
    }
}
