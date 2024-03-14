using System.Diagnostics;
using Apache.NMS;
using Apache.NMS.AMQP;
using Apache.NMS.AMQP.Message;
using Apache.NMS.AMQP.Util;
using Apache.NMS.Util;
using Messaging09.Amqp.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tracer = Messaging09.Amqp.Tracing.Tracer;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public sealed class ScopedMessageListener<TMessageType> : IListener, IDisposable
  where TMessageType : class
{
  private readonly ILogger<ScopedMessageListener<TMessageType>> _logger;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ISessionFactory _sessionFactory;
  private readonly MessagingConfig _messagingConfig;
  private readonly string _queue;
  private IMessageConsumer? _consumer;
  private ActivitySource _actSource = new("Messaging");

  public ScopedMessageListener(ILogger<ScopedMessageListener<TMessageType>> logger,
    IServiceScopeFactory scopeFactory,
    ISessionFactory sessionFactory,
    MessagingConfig messagingConfig, string queue)
  {
    _logger = logger;
    _scopeFactory = scopeFactory;
    _sessionFactory = sessionFactory;
    _messagingConfig = messagingConfig;
    _queue = queue;
  }

  public async Task StartListening(string? selector = null)
  {
    var session = await _sessionFactory.GetSession();
    var destination = SessionUtil.GetDestination(session, _queue);
    _consumer = await session.CreateConsumerAsync(destination, selector);
    if (_consumer == null) throw new Exception($"could not create consumer for {_queue}");
    _consumer.Listener += HandleMessage;
  }

  private async void HandleMessage(IMessage message)
  {
    var outcome = _messagingConfig.DefaultAck;
    using var scope = _scopeFactory.CreateScope();
    var correlationContextAccessor = scope.ServiceProvider.GetRequiredService<CorrelationContextAccessor>();
    var handler = scope.ServiceProvider.GetRequiredService<MessageHandler<TMessageType>>();
    var pluginChain = new PluginChain(scope.ServiceProvider.GetRequiredService<IEnumerable<MessagingPlugin>>(),
      _messagingConfig);

    using var activity = StartActivity(message, handler.GetType().Name);

    var correlationId = message.NMSCorrelationID ?? Guid.NewGuid().ToString("D");
    correlationContextAccessor.CorrelationId = correlationId;
    using var logScope = SetupLogScope(message, correlationId);

    try
    {
      // Tracer.InfoFormat("received message on {0}, executing handler", _queue);
      outcome = await pluginChain.HandleInbound(message, handler);
    }
    catch (Exception e)
    {
      Tracer.ErrorFormat("unhandled exception in message listener. was not catched by errorHandlingPlugin: {0}",
        e.ToString());
      outcome = _messagingConfig.GetOutcomeForException(e);
    }
    finally
    {
      Tracer.InfoFormat("Acking messageId {0} with {1}", message.NMSMessageId, outcome);
      SetMessageAckType(outcome, message);
      await message.AcknowledgeAsync();
    }
  }


  private static void SetMessageAckType(MessageOutcome outcome, IMessage message)
  {
    message.Properties["NMS_AMQP_ACK_TYPE"] = outcome switch
    {
      MessageOutcome.Ack => AckType.ACCEPTED,
      MessageOutcome.Failed => AckType.MODIFIED_FAILED,
      MessageOutcome.FailedUndeliverable => AckType.MODIFIED_FAILED_UNDELIVERABLE,
      MessageOutcome.Reject => AckType.REJECTED,
      _ => throw new ArgumentOutOfRangeException(nameof(outcome))
    };
  }

  private Activity? StartActivity(IMessage message, string handlerName)
  {
    var parentTraceId = message.Properties.Contains(Constants.PARENT_TRACE_ID_KEY)
      ? message.Properties.GetString(Constants.PARENT_TRACE_ID_KEY)
      : "";
    var parentSpanId = message.Properties.Contains(Constants.PARENT_SPAN_ID_KEY)
      ? message.Properties.GetString(Constants.PARENT_SPAN_ID_KEY)
      : "";

    ActivityContext parent;

    if (!string.IsNullOrWhiteSpace(parentTraceId) && !string.IsNullOrWhiteSpace(parentSpanId))
    {
      parent = new ActivityContext(ActivityTraceId.CreateFromString(parentTraceId),
        ActivitySpanId.CreateFromString(parentSpanId), ActivityTraceFlags.Recorded, null, true);
    }

    return _actSource.StartActivity(
      $"Message received: {handlerName}",
      ActivityKind.Consumer,
      parent,
      new KeyValuePair<string, object?>[]
      {
        new("messaging.system", "Amqp"),
        new("messaging.destination", message.NMSDestination.ToString())
      });
  }


  public async Task StopListening()
  {
    if (_consumer != null)
    {
      await _consumer.CloseAsync();
    }
  }

  public void Dispose()
  {
    _consumer?.Dispose();
  }

  private MessageLogScope SetupLogScope(IMessage message, string correlationId)
  {
    return new MessageLogScope()
    {
      MessageLoggerScope = _logger.BeginScope(new Dictionary<string, object>()
      {
        { "CorrelationId", correlationId },
        { "MessageType", message.NMSType },
        { "MessageDestination", message.NMSDestination }
      })
    };
  }

  private class MessageLogScope : IDisposable
  {
    public IDisposable? MessageLoggerScope { get; init; }

    public void Dispose()
    {
      MessageLoggerScope?.Dispose();
    }
  }
}
