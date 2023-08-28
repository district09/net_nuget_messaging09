using Messaging09.Amqp.Serializers;
using Xunit.Abstractions;

namespace Messaging09.Amqp.Test.TestClasses;

public class TestHandler : MessageHandler<string>
{
    private readonly Func<MessageEnvelope<string>, MessageOutcome> _handler;

    public TestHandler(IMessageSerializer<string> serializer, Func<MessageEnvelope<string>, MessageOutcome> handler) :
        base(serializer)
    {
        _handler = handler;
    }

    protected override  Task<MessageOutcome> Handle(MessageEnvelope<string> envelope)
    {
        return Task.FromResult(_handler.Invoke(envelope));
    }
}
