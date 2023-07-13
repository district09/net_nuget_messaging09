using Apache.NMS;
using Apache.NMS.AMQP.Message.Facade;
using Apache.NMS.Util;

namespace Messaging09.Amqp.Test.Message;

public class NmsTestMapMessageFacade : NmsTestMessageFacade, INmsMapMessageFacade
{
    public IPrimitiveMap Map { get; } = new PrimitiveMap();
}