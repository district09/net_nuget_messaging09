using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace SelectorExample;

public class DlqMessageHandler(IMessageSerializer<DlqMessageToFail> serializer, ILogger<DlqMessageHandler> logger)
  : MessageHandler<DlqMessageToFail>(serializer)
{
  protected override async Task<MessageOutcome> Handle(MessageEnvelope<DlqMessageToFail> envelope)
  {
    if (envelope.Message.Value != 2)
    {
      logger.LogError(new ArgumentException(nameof(envelope.Message.Value)), "message value was not 2");
      return MessageOutcome.Reject;
    }

    logger.LogInformation("Received DLQ message with value: {Message}", envelope.Message.Value);
    await Task.Delay(100);
    return MessageOutcome.Ack;
  }
}
