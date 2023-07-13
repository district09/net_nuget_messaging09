using Apache.NMS;
using Messaging09.Amqp.Serializers;
using Messaging09.Amqp.Test.Message;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Messaging09.Amqp.Test;

public class TestPlugin : MessagingPlugin
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly int _order;

    public TestPlugin(ITestOutputHelper outputHelper, int order)
    {
        _outputHelper = outputHelper;
        _order = order;
    }

    public override Task<MessageOutcome> HandleInbound<TMessageType>(IMessage message,
        MessageHandler<TMessageType> handler)
    {
        _outputHelper.WriteLine("test output {0}", _order);
        return base.HandleInbound(message, handler);
    }
}

public class TestHandler : MessageHandler<string>
{
    private readonly ITestOutputHelper _outputHelper;

    public TestHandler(IMessageSerializer<string> serializer, ITestOutputHelper outputHelper) : base(serializer)
    {
        _outputHelper = outputHelper;
    }

    protected override async Task<MessageOutcome> Handle(MessageEnvelope<string> envelope)
    {
        _outputHelper.WriteLine("message content: {0}", envelope.Message);
        return MessageOutcome.Ack;
    }
}

public class TestSerializer : IMessageSerializer<string>
{
    public string Deserialize(IMessage message)
    {
        return ((ITextMessage)message).Text;
    }

    public IMessage Serialize(string message, ISession session)
    {
        var x = new TestMessageFactory();
        return x.CreateTextMessage(message);
    }
}