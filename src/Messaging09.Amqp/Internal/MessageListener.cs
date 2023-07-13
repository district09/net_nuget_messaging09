using Apache.NMS;
using Apache.NMS.AMQP.Message;
using Apache.NMS.Util;
using Messaging09.Amqp.Config;
using Tracer = Messaging09.Amqp.Tracing.Tracer;

namespace Messaging09.Amqp.Internal;

public sealed class MessageListener<TMessageType> : IListener, IDisposable
    where TMessageType : class
{
    private readonly ISessionFactory _sessionFactory;
    private readonly MessageHandlingConfig _config;
    private readonly MessagingPlugin _pluginChain;
    private readonly MessageHandler<TMessageType> _handler;
    private IMessageConsumer? _consumer;

    public MessageListener(
        ISessionFactory sessionFactory,
        MessageHandlingConfig config,
        MessagingPlugin pluginChain,
        MessageHandler<TMessageType> handler)
    {
        _sessionFactory = sessionFactory;
        _config = config;
        _pluginChain = pluginChain;
        _handler = handler;
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
        var outcome = _config.DefaultAck;

        try
        {
            Tracer.DebugFormat("handling incoming message from {0}", message.NMSDestination);
            outcome = await _pluginChain.HandleInbound(message, _handler);
        }
        catch (Exception e)
        {
            Tracer.ErrorFormat("an unhandled exception occured while handling message: {0}", e.Message);
            outcome = _config.GetOutcomeForException(e);
        }
        finally
        {
            Tracer.DebugFormat("Acking with {AckType}", outcome.ToString());
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