using System.Diagnostics;
using Apache.NMS;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.IntegrationTests.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Messaging09.Amqp.IntegrationTests;

[Collection("IntegrationTests")]
public class AliceBobTests : IDisposable
{
  private readonly BobFixture _bobFixture;
  private readonly AliceFixture _aliceFixture;
  private readonly ITestOutputHelper _output;

  public AliceBobTests(BobFixture bobFixture, AliceFixture aliceFixture, ITestOutputHelper output)
  {
    _bobFixture = bobFixture;
    _aliceFixture = aliceFixture;
    _output = output;
    // var tracer = new NmsTestTracer(_output)
    // {
    //   IsDebugEnabled = false
    // };
    // Tracer.Trace = tracer;
    bobFixture.Messages?.Reset();
    aliceFixture.Messages?.Reset();

    BrokerTestUtils.PurgeQueue("Messaging09.Amqp.IntegrationTests.BobMessage");
  }

  [Fact]
  public async Task Message_OverQueue_Succeeds()
  {
    await _bobFixture.SendMessage(new BobMessage(), "Messaging09.Amqp.IntegrationTests.BobMessage");

    await WaitForMessages(1, 3);

    Assert.Equal(1, _aliceFixture.Messages?.Incoming.Count);
  }

  [Fact]
  public async Task Message_OverTopic_Succeeds()
  {
    await _bobFixture.SendMessage(new BobMessage(),
      "VirtualTopic.Messaging09.Amqp.IntegrationTests.BobMessage");

    await WaitForMessages(10, 2);

    Assert.Equal(1, _aliceFixture.Messages?.Incoming.Count);
  }

  [Fact]
  public async Task Message_ConsumerCrashes_IsRedelivered()
  {
    await _bobFixture.SendMessage(new BobMessage
    {
      ConsumerShouldCrash = true
    }, "Messaging09.Amqp.IntegrationTests.BobMessage");

    // Default max-delivery-attempts is eg 10, so we're expecting 10
    // However, we need to wait (until timeout) to see if we're not receiving 11 deliveries of the message
    await WaitForMessages(BrokerTestUtils.BrokerMaxDeliveryAttempts + 1, 5);

    BrokerTestUtils.AssertQueueIsEmpty("Messaging09.Amqp.IntegrationTests.BobMessage");
    BrokerTestUtils.AssertQueueSize("DLQ", 1);

    Assert.Equal(BrokerTestUtils.BrokerMaxDeliveryAttempts, _aliceFixture.Messages?.Incoming.Count);
  }

  [Fact]
  public async Task MultipleLongRunningMessages_AreProcessedParallelWithinPrefetchSize()
  {
    int? prefetchSize = _aliceFixture.Services.GetRequiredService<IOptions<BrokerOptions>>().Value.PrefetchPolicy;

    Assert.NotNull(prefetchSize);

    const int numberOfSentMessages = 40;

    for (var i = 0; i < numberOfSentMessages; i++)
    {
      await _bobFixture.SendMessage(new BobMessage
      {
        ConsumerIsSlow = true
      }, "Messaging09.Amqp.IntegrationTests.BobMessage");
    }

    await WaitForMessages(numberOfSentMessages + 1, 10);

    Assert.Equal(prefetchSize, _aliceFixture.Messages?.MaxConcurrentCount);
    Assert.Equal(numberOfSentMessages, _aliceFixture.Messages?.Incoming.Count);

    BrokerTestUtils.AssertQueueIsEmpty("Messaging09.Amqp.IntegrationTests.BobMessage");
    BrokerTestUtils.AssertQueueIsEmpty("DLQ");
  }

  [Fact]
  public async Task MultipleLongRunningMessages_AndRetries_AreProcessedParallelWithinPrefetchSize()
  {
    int? prefetchSize = _aliceFixture.Services.GetRequiredService<IOptions<BrokerOptions>>().Value.PrefetchPolicy;

    Assert.NotNull(prefetchSize);

    var messageNumber = 0;
    var numberOfMessages = 0;

    const int numberOfFailingMessages = 2;
    for (var i = 0; i < numberOfFailingMessages; i++)
    {
      await _bobFixture.SendMessage(new BobMessage
      {
        MessageNumber = messageNumber++,
        ConsumerShouldCrash = true
      }, "Messaging09.Amqp.IntegrationTests.BobMessage");
    }

    numberOfMessages += numberOfFailingMessages * BrokerTestUtils.BrokerMaxDeliveryAttempts;

    const int numberOfSlowMessages = 15;
    for (var i = 0; i < numberOfSlowMessages; i++)
    {
      await _bobFixture.SendMessage(new BobMessage
      {
        MessageNumber = messageNumber++,
        ConsumerIsSlow = true
      }, "Messaging09.Amqp.IntegrationTests.BobMessage");
    }

    numberOfMessages += numberOfSlowMessages;

    _output.WriteLine($"Prefetch size: {prefetchSize}");
    _output.WriteLine(
      $"Number of failing messages: {numberOfFailingMessages} * {BrokerTestUtils.BrokerMaxDeliveryAttempts} retries = {numberOfFailingMessages * BrokerTestUtils.BrokerMaxDeliveryAttempts}");
    _output.WriteLine($"Number of slow messages: {numberOfSlowMessages}");
    _output.WriteLine($"Number of expected messages: {numberOfMessages}");

    await WaitForMessages(numberOfMessages + 1, 5);

    Assert.Equal(numberOfMessages, _aliceFixture.Messages?.Incoming.Count);
    Assert.True(_aliceFixture.Messages?.MaxConcurrentCount < prefetchSize);

    BrokerTestUtils.AssertQueueIsEmpty("Messaging09.Amqp.IntegrationTests.BobMessage");
    BrokerTestUtils.AssertQueueSize("DLQ", numberOfFailingMessages);
  }

  [Fact]
  public async Task MultipleParallelSends_UseSameConnection()
  {
    const int numberOfMessages = 10;
    var tasks = new Task[numberOfMessages];
    for (var i = 0; i < numberOfMessages; i++)
    {
      var t = _bobFixture.SendMessage(new BobMessage
      {
        MessageNumber = i
      }, "Messaging09.Amqp.IntegrationTests.BobMessage");
      tasks[i] = t;
    }

    await Task.WhenAll(tasks);

    await WaitForMessages(numberOfMessages + 1, 1);

    Assert.Equal(numberOfMessages, _aliceFixture.Messages?.Incoming.Count);
  }


  [DebuggerStepThrough]
  private async Task WaitForMessages(int amount, int timeoutSeconds)
  {
    await Task.WhenAny(Task.Run(async () =>
    {
      while (_aliceFixture.Messages?.Incoming.Count < amount)
      {
        await Task.Delay(100);
      }
    }), Task.Delay(TimeSpan.FromSeconds(timeoutSeconds)));
  }

  public void Dispose()
  {
    BrokerTestUtils.PurgeQueue("Messaging09.Amqp.IntegrationTests.BobMessage");
    BrokerTestUtils.PurgeQueue("DLQ");
  }
}
