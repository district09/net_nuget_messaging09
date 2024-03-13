using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace SelectorExample;

public class FailHandler(IMessageSerializer<MessageToFail> serializer, ILogger<FailHandler> logger)
  : MessageHandler<MessageToFail>(serializer)
{
  protected override async Task<MessageOutcome> Handle(MessageEnvelope<MessageToFail> envelope)
  {
    await Task.Delay(200);
    logger.LogInformation("Rejecting message {Message}", envelope.Message.Value);
    return MessageOutcome.Reject;
  }
}
