using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace SelectorExample;

public class SecondQueueFailHandler(
  IMessageSerializer<SecondMessageToFail> serializer,
  ILogger<SecondQueueFailHandler> logger)
  : MessageHandler<SecondMessageToFail>(serializer)
{
  protected override async Task<MessageOutcome> Handle(MessageEnvelope<SecondMessageToFail> envelope)
  {
    await Task.Delay(200);
    logger.LogInformation("Rejecting message {Message}", envelope.Message.Value);
    return MessageOutcome.Reject;
  }
}
