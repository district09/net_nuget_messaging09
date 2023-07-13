using Messaging09.Amqp.Test.Message;
using Xunit.Abstractions;

namespace Messaging09.Amqp.Test;

public class UnitTest1
{
    private ITestOutputHelper _outputHelper;
    private TestMessageFactory _messageFactory = new();

    public UnitTest1(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task Test1()
    {
        var plugin = new TestPlugin(_outputHelper, 1);
        plugin.SetNext(new TestPlugin(_outputHelper, 2));
        plugin.SetNext(new TestPlugin(_outputHelper, 3));

        var msg = _messageFactory.CreateTextMessage("0");
        var outcome = await plugin.HandleInbound(msg, new TestHandler(new TestSerializer(), _outputHelper));
        Assert.Equal(MessageOutcome.Ack, outcome);
    }
}