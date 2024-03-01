using Apache.NMS;
using Apache.NMS.AMQP;
using Apache.NMS.AMQP.Message;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Messaging09.Amqp.IntegrationTests;

public class AmqpTests : IDisposable
{
  readonly IConnection _connection;
  readonly string _testName = "Messaging09.Amqp.IntegrationTests.AmqpTests";
  private readonly ISession _session;
  private readonly ITestOutputHelper _logger;

  public AmqpTests(ITestOutputHelper outputHelper)
  {
    BrokerTestUtils.PurgeQueue(TimeSpan.FromMilliseconds(500), _testName);
    BrokerTestUtils.PurgeQueue(TimeSpan.FromMilliseconds(500), "DLQ");

    _connection = BrokerTestUtils.CreateConnection();
    _connection.Start();

    _session = _connection.CreateSession(AcknowledgementMode.IndividualAcknowledge);
    _logger = outputHelper;
   // Tracer.Trace = new NmsTestTracer(_logger);
  }

  [Fact]
  public void Queue_WorksAsExpected()
  {
    var queue = _session.GetQueue(_testName);

    // Produce
    var producer = _session.CreateProducer(queue);
    var msgSent = _session.CreateTextMessage("Hello");
    producer.Send(msgSent, MsgDeliveryMode.Persistent, MsgPriority.Normal, TimeSpan.Zero);
    producer.Close();

    // Consume
    var messageConsumer = _session.CreateConsumer(queue);
    var msgReceived = messageConsumer.Receive(TimeSpan.FromSeconds(1));
    msgReceived.Acknowledge();

    Assert.NotNull(msgReceived);
    BrokerTestUtils.AssertQueueIsEmpty(TimeSpan.FromMilliseconds(500), _testName);
  }

  [Fact]
  public void Redelivery_WorksAsExpected()
  {
    var queue = _session.GetQueue(_testName);

    // Produce
    var producer = _session.CreateProducer(queue);
    var msgSent = _session.CreateTextMessage("Hello");
    producer.Send(msgSent, MsgDeliveryMode.Persistent, MsgPriority.Normal, TimeSpan.Zero);
    producer.Close();
    producer.Dispose();
    _logger.WriteLine("Message sent");

    // Consumer
    var messageConsumer = _session.CreateConsumer(queue);
    _logger.WriteLine("Consumer created");
    for (var i = 0; i < BrokerTestUtils.BrokerMaxDeliveryAttempts; i++)
    {
      // Consume & crash
      var msgReceived = messageConsumer.Receive(TimeSpan.FromSeconds(1));
      _logger.WriteLine($"Received message: {msgReceived}");
      ((NmsMessage)msgReceived).NmsAcknowledgeCallback = new NmsAcknowledgeCallback((NmsSession)_session)
      {
        AcknowledgementType = AckType.MODIFIED_FAILED
      };
      msgReceived.Acknowledge();
    }


    // Verify that the message is not redelivered after max delivery attempts
    var nonExistentMessage = messageConsumer.Receive(TimeSpan.FromSeconds(1));
    _logger.WriteLine($"Received message should be null: {nonExistentMessage == null}");
    Assert.Null(nonExistentMessage);
    messageConsumer.Close();
    messageConsumer.Dispose();

    // Verify that the DLQ contains the message
    BrokerTestUtils.AssertQueueSize(TimeSpan.FromSeconds(1), "DLQ", 1);
  }

  public void Dispose()
  {
    _connection.Close();
    _connection.Dispose();
  }
}
