using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace NoDIWorkerExample;

public class CustomHandler : MessageHandler<PingMessage>
{
    public CustomHandler(IMessageSerializer<PingMessage> serializer) : base(serializer)
    {
    }

    protected override Task<MessageOutcome> Handle(MessageEnvelope<PingMessage> envelope)
    {
        Console.WriteLine($"Message arrived with message: {envelope.Message.Ping}");
        return Task.FromResult(MessageOutcome.Ack);
    }
}