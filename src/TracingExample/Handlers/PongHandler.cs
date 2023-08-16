using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;
using TracingExample.Viewmodels;

namespace TracingExample.Handlers;

public class PongHandler : MessageHandler<PongMessage>
{
    private readonly IMessagePublisher _publisher;

    public PongHandler(IMessageSerializer<PongMessage> serializer, IMessagePublisher publisher) : base(serializer)
    {
        _publisher = publisher;
    }

    protected override async Task<MessageOutcome> Handle(MessageEnvelope<PongMessage> envelope)
    {
        if (envelope.Message.PingCount < 10)
        {
            await _publisher.SendMessage(new PingMessage() { PingCount = envelope.Message.PingCount + 1 },
                "ping.queue");
        }

        return MessageOutcome.Ack;
    }
}