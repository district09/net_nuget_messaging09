using Apache.NMS;
using Apache.NMS.AMQP.Message;
using Apache.NMS.AMQP.Message.Facade;
using Apache.NMS.Util;

namespace Messaging09.Amqp.Test.Message;

[Serializable]
public class NmsTestMessageFacade : INmsMessageFacade
{
    public virtual NmsMessage AsMessage()
    {
        return new NmsMessage(this);
    }

    public virtual void ClearBody()
    {
    }

    public int DeliveryCount { get; set; }

    public int RedeliveryCount { get; set; }

    public void OnSend(TimeSpan producerTtl)
    {
            
    }

    public string NMSMessageId
    {
        get => ProviderMessageIdObject as string;
        set => ProviderMessageIdObject = value;
    }

    public IPrimitiveMap Properties { get; } = new PrimitiveMap();
    public string NMSCorrelationID { get; set; }
    public IDestination NMSDestination { get; set; }
    public TimeSpan NMSTimeToLive { get; set; }
    public MsgPriority NMSPriority { get; set; }
    public bool NMSRedelivered { get; set; }
    public IDestination NMSReplyTo { get; set; }
    public DateTime NMSTimestamp { get; set; }
    public string NMSType { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string GroupId { get; set; }
    public uint GroupSequence { get; set; }
    public DateTime? Expiration { get; set; }
    public sbyte? JmsMsgType { get; }
    public bool IsPersistent { get; set; }
    public object ProviderMessageIdObject { get; set; }

    public INmsMessageFacade Copy()
    {
        return null;
    }

    public bool HasBody()
    {
        return true;
    }
}