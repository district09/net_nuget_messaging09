using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.IntegrationTests.Setup;

public class MessageTracker
{
  private readonly ILogger<MessageTracker> _logger;

  public MessageTracker(ILogger<MessageTracker> logger)
  {
    _logger = logger;
  }

  private int _maxConcurrentCount;
  private readonly object _maxConcurrentCountLock = new();

  public ConcurrentBag<IMessage> Incoming { get; } = new();

  public int MaxConcurrentCount => _maxConcurrentCount;

  public void UpdateConcurrentCount(string message, int concurrentCount)
  {
    // Keep track of the max concurrent running consumers
    lock (_maxConcurrentCountLock)
    {
      _logger.LogInformation("Concurrent: {Count} - {Message}", concurrentCount, message);
      _maxConcurrentCount = Math.Max(concurrentCount, _maxConcurrentCount);
    }
  }

  public void Reset()
  {
    _maxConcurrentCount = 0;
    Incoming.Clear();
  }
}
