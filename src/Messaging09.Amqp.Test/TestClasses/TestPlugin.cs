using Apache.NMS;
using Xunit.Abstractions;

namespace Messaging09.Amqp.Test.TestClasses;

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
        _outputHelper.WriteLine("executing test plugin {0}", _order);
        return base.HandleInbound(message, handler);
    }
}