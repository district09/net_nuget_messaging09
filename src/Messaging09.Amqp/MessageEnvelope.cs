using Apache.NMS;
using Apache.NMS.AMQP;

namespace Messaging09.Amqp;

public class MessageEnvelope<TMessageType>
{
  public MessageEnvelope(IMessage message, TMessageType parsed)
  {
    Original = message;
    Message = parsed;
  }

  public IMessage Original { get; init; }
  public TMessageType Message { get; init; }

  public string? ReplyTo => Original.NMSReplyTo is { IsQueue: true }
    ? ((NmsQueue?)Original.NMSReplyTo)?.QueueName
    : ((NmsTopic?)
      Original.NMSReplyTo)?.TopicName;
}
