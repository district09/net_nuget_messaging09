using Messaging09.Amqp.Config;
using Messaging09.Amqp.Test.Message;
using Messaging09.Amqp.Test.TestClasses;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Messaging09.Amqp.Test;

public class PluginChainTests
{
    private readonly TestMessageFactory _testMessageFactory = new();
    private readonly ITestOutputHelper _outputHelper;

    public PluginChainTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task KnownException_ShouldReturnCorrectAckType()
    {
        IEnumerable<MessagingPlugin> plugins = new[]
        {
            new TestPlugin(_outputHelper, 5)
        };
        var config = new MessageHandlingConfig
        {
            DefaultAck = MessageOutcome.FailedUndeliverable,
            UnhandledExceptionAck = MessageOutcome.Reject
        };
        config.AddKnownException<TestException>(MessageOutcome.Failed);

        var pluginChain = new PluginChain(plugins, config);

        var handler = new TestHandler(new TestSerializer(), _ => throw new TestException());

        var result = await pluginChain.HandleInbound(_testMessageFactory.CreateTextMessage("test"), handler);
        Assert.Equal(MessageOutcome.Failed, result);
    }

    [Fact]
    public async Task UnknownException_ShouldReturnUnhandledExceptionAckType()
    {
        IEnumerable<MessagingPlugin> plugins = new[]
        {
            new TestPlugin(_outputHelper, 5)
        };
        var config = new MessageHandlingConfig
        {
            DefaultAck = MessageOutcome.FailedUndeliverable,
            UnhandledExceptionAck = MessageOutcome.Reject
        };
        config.AddKnownException<TestException>(MessageOutcome.Failed);

        var pluginChain = new PluginChain(plugins, config);

        var handler = new TestHandler(new TestSerializer(), _ => throw new Exception("unknown exception"));

        var result = await pluginChain.HandleInbound(_testMessageFactory.CreateTextMessage("test"), handler);
        Assert.Equal(MessageOutcome.Reject, result);
    }
}