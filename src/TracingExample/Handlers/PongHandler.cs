using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;
using TracingExample.Viewmodels;

namespace TracingExample.Handlers;

public class PongHandler : MessageHandler<PongMessage>
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<PongHandler> _logger;

    public PongHandler(
        IMessageSerializer<PongMessage> serializer,
        IMessagePublisher publisher,
        ILogger<PongHandler> logger) : base(serializer)
    {
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task<MessageOutcome> Handle(MessageEnvelope<PongMessage> envelope)
    {
        _logger.LogInformation("received pongmessage with count {Count}", envelope.Message.PingCount);
        if (envelope.Message.PingCount >= 10) return MessageOutcome.Ack;
        await Task.Delay(500);
        await _publisher.SendMessage(new PingMessage() { PingCount = envelope.Message.PingCount + 1 },
            "ping.queue");

        return MessageOutcome.Ack;
    }
}
