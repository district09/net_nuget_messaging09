using System.Diagnostics;
using Apache.NMS;
using Apache.NMS.Util;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class CorrelatedMessagePublisher : IMessagePublisher
{
    private readonly ISessionFactory _sessionFactory;
    private readonly PluginChain _pluginChain;
    private readonly CorrelationContextAccessor _contextAccessor;
    private readonly IServiceProvider _provider;
    private readonly MessagingConfig _config;
    private readonly ActivitySource _actSource = new("Messaging");

    public CorrelatedMessagePublisher(ISessionFactory sessionFactory,
        PluginChain pluginChain,
        CorrelationContextAccessor contextAccessor,
        IServiceProvider provider,
        MessagingConfig config)
    {
        _sessionFactory = sessionFactory;
        _pluginChain = pluginChain;
        _contextAccessor = contextAccessor;
        _provider = provider;
        _config = config;
    }

    public async Task SendMessage<TMessageType>(TMessageType message, string queue,
        Action<IMessage>? messageTransform = null)
    {
        var session = await _sessionFactory.GetSession();
        var isTopic = queue.StartsWith(_config.TopicPrefix);
        using var dest =
            SessionUtil.GetDestination(session, queue, isTopic ? DestinationType.Topic : DestinationType.Queue);
        using var producer = await session.CreateProducerAsync(dest);

        var serializer = _provider.GetRequiredService<IMessageSerializer<TMessageType>>();

        var outboundMessage = serializer.Serialize(message, session);
        SetupTracing(outboundMessage, queue);
        messageTransform?.Invoke(outboundMessage);
        var msg = await _pluginChain.HandleOutbound(outboundMessage, dest);
        Tracer.InfoFormat("sending message to {0}", queue);
        await producer.SendAsync(dest, msg);
    }

    private void SetupTracing(IMessage msg, string queue)
    {
        using var activity = _actSource.StartActivity($"Send message to {queue}", ActivityKind.Producer,
            default(ActivityContext), new KeyValuePair<string, object?>[]
            {
                new("messaging.system", "Amqp"),
                new("messaging.destination", queue)
            });

        msg.Properties.SetString(Constants.PARENT_SPAN_ID_KEY, activity?.SpanId.ToHexString());
        msg.Properties.SetString(Constants.PARENT_TRACE_ID_KEY, activity?.TraceId.ToHexString());

        msg.NMSCorrelationID = _contextAccessor.CorrelationId;
    }
}