using Apache.NMS;
using Apache.NMS.AMQP;

namespace Messaging09.Amqp.IntegrationTests;

public static class BrokerTestUtils
{
  public const int BrokerMaxDeliveryAttempts = 3;

  public static IConnection CreateConnection()
  {
    var factory = new NmsConnectionFactory("amqp://localhost:5672");
    var connection = factory.CreateConnection("artemisuser", "artemispassword"); // TODO: change this
    return connection;
  }

  public static void AssertQueueIsEmpty(string queueName)
  {
    AssertQueueIsEmpty(TimeSpan.FromSeconds(1), queueName);
  }

  public static void AssertQueueIsEmpty(TimeSpan timeout, string queueName)
  {
    var connection = CreateConnection();
    connection.Start();
    using var session = connection.CreateSession();
    var queue = session.GetQueue(queueName);
    using var consumer = session.CreateConsumer(queue);
    var message = consumer.Receive(timeout);
    Assert.Null(message);

    connection.Close();
  }

  public static void AssertQueueSize(string queueName, int expectedCount)
  {
    AssertQueueSize(TimeSpan.FromSeconds(1), queueName, expectedCount);
  }

  public static void AssertQueueSize(TimeSpan timeout, string queueName, int expectedCount)
  {
    var amqpConnection = CreateConnection();
    amqpConnection.Start();
    var session = amqpConnection.CreateSession(AcknowledgementMode.IndividualAcknowledge);
    var queue = session.GetQueue(queueName);
    var consumer = session.CreateConsumer(queue);

    var count = 0;
    IMessage message;
    do
    {
      message = consumer.Receive(timeout);
      if (message != null)
        count++;
    } while (message != null);

    consumer.Close();
    consumer.Dispose();
    amqpConnection.Close();
    amqpConnection.Dispose();

    Assert.Equal(expectedCount, count);
  }

  public static void PurgeQueue(string queueName)
  {
    PurgeQueue(TimeSpan.FromSeconds(1), queueName);
  }

  public static void PurgeQueue(TimeSpan timeout, string queueName)
  {
    var amqpConnection = CreateConnection();
    amqpConnection.Start();
    var session = amqpConnection.CreateSession(AcknowledgementMode.AutoAcknowledge);
    var queue = session.GetQueue(queueName);
    var consumer = session.CreateConsumer(queue);

    IMessage message;
    do
    {
      message = consumer.Receive(timeout);
    } while (message != null);

    amqpConnection.Close();
  }
}
