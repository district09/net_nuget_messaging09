using Apache.NMS.AMQP.Message.Facade;

namespace Messaging09.Amqp.Test.Message;

public class NmsTestObjectMessageFacade : NmsTestMessageFacade, INmsObjectMessageFacade
{
    public object Object { get; set; }

    public override void ClearBody()
    {
        Object = null;
    }
}
