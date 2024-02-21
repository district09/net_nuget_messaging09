using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace ValidationExample;

public class Handler : MessageHandler<ValidatedMessage>
{
  public Handler(IMessageSerializer<ValidatedMessage> serializer) : base(serializer)
  {
  }

  protected override async Task<MessageOutcome> Handle(MessageEnvelope<ValidatedMessage> envelope)
  {
    return MessageOutcome.Ack;
  }
}
