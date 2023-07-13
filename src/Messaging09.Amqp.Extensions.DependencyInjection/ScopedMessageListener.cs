using Apache.NMS;
using Apache.NMS.AMQP.Message;
using Apache.NMS.Util;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.Plugins;
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
    private readonly MessageHandlingConfig _messageHandlingConfig;
    private IMessageConsumer? _consumer;

    public ScopedMessageListener(
        ILogger<ScopedMessageListener<TMessageType>> logger,
        IServiceScopeFactory scopeFactory,
        ISessionFactory sessionFactory,
        MessageHandlingConfig messageHandlingConfig)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sessionFactory = sessionFactory;
        _messageHandlingConfig = messageHandlingConfig;
    }

    public async Task StartListening(string destinationName, string? selector = null)
    {
        var session = await _sessionFactory.GetSession();
        var destination = SessionUtil.GetDestination(session, destinationName);
        _consumer = await session.CreateConsumerAsync(destination, selector);
        if (_consumer == null) throw new Exception($"could not create consumer for {destinationName}");
        _consumer.Listener += HandleMessage;
    }

    private async void HandleMessage(IMessage message)
    {
        var outcome = _messageHandlingConfig.DefaultAck;
        using var scope = _scopeFactory.CreateScope();
        var correlationContextAccessor = scope.ServiceProvider.GetRequiredService<CorrelationContextAccessor>();
        correlationContextAccessor.CorrelationId = message.NMSCorrelationID ?? Guid.NewGuid().ToString("D");
        using var correlationScope = _logger.BeginScope(new Dictionary<string, object>
            { { "CorrelationId", correlationContextAccessor.CorrelationId } });
        try
        {
            var handler = scope.ServiceProvider.GetRequiredService<MessageHandler<TMessageType>>();

            var pluginChain = GetPluginChain(scope);

            outcome = await pluginChain.HandleInbound(message, handler);
        }
        catch (Exception e)
        {
            Tracer.ErrorFormat("unhandled exception in message listener. was not catched by errorHandlingPlugin: {0}",
                e.ToString());
            outcome = _messageHandlingConfig.GetOutcomeForException(e);
        }
        finally
        {
            _logger.LogInformation("Acking with {AckType}", outcome);
            SetMessageAckType(outcome, message);
            await message.AcknowledgeAsync();
        }
    }

    private MessagingPlugin GetPluginChain(IServiceScope scope)
    {
        var plugins = scope.ServiceProvider.GetRequiredService<IEnumerable<MessagingPlugin>>();

        var first = new ErrorHandlingPlugin(_messageHandlingConfig);

        foreach (var plugin in plugins)
        {
            first.SetNext(plugin);
        }

        return first;
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
}