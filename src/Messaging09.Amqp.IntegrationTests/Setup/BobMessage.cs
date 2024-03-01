namespace Messaging09.Amqp.IntegrationTests.Setup;

public class BobMessage : IMessage
{
  public int MessageNumber { get; set; }

  public bool ConsumerShouldCrash { get; set; }

  public bool ConsumerIsSlow { get; set; }
}
