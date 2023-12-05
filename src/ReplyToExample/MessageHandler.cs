using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace ReplyToExample;

public class MessageHandler : MessageHandler<MessageDto>
{
  private readonly IMessagePublisher _publisher;
  private readonly ILogger<MessageHandler> _logger;
  private const string DefaultReplyTopic = "default.reply.queue";

  public MessageHandler(IMessageSerializer<MessageDto> serializer, IMessagePublisher publisher,
    ILogger<MessageHandler> logger) : base(serializer)
  {
    _publisher = publisher;
    _logger = logger;
  }

  protected override async Task<MessageOutcome> Handle(MessageEnvelope<MessageDto> envelope)
  {
    var replyTo = envelope.ReplyTo ?? DefaultReplyTopic;

    _logger.LogInformation("received message {Count}: sending to {ReplyTo}", envelope.Message.Count, replyTo);

    await _publisher.SendMessage(envelope.Message, replyTo);

    return MessageOutcome.Ack;
  }
}
