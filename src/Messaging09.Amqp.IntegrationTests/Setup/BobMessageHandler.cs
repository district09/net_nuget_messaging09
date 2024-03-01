using System.Text.Json;
using Messaging09.Amqp.Serializers;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.IntegrationTests.Setup;

public class BobMessageHandler : MessageHandler<BobMessage>
{
  private static readonly SemaphoreSlim Semaphore = new(InitialCount);
  private const int InitialCount = 1000;
  private readonly MessageTracker _messages;
  private readonly ILogger<BobMessageHandler> _logger;

  public BobMessageHandler(IMessageSerializer<BobMessage> serializer, MessageTracker messages,
    ILogger<BobMessageHandler> logger) : base(serializer)
  {
    _messages = messages;
    _logger = logger;
  }

  protected override async Task<MessageOutcome> Handle(MessageEnvelope<BobMessage> envelope)
  {
    try
    {
      await Semaphore.WaitAsync();

      // Update the number of current enters in the semaphore
      _messages.UpdateConcurrentCount(JsonSerializer.Serialize(envelope.Message),
        InitialCount - Semaphore.CurrentCount);

      _messages.Incoming.Add(envelope.Message);

      if (envelope.Message.ConsumerShouldCrash)
      {
        throw new InvalidOperationException();
      }

      if (envelope.Message.ConsumerIsSlow)
      {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
      }
    }
    finally
    {
      Semaphore.Release();
    }

    return MessageOutcome.Ack;
  }
}
