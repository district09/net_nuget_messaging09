using System.Diagnostics;
using Apache.NMS;
using Apache.NMS.Util;
using Messaging09.Amqp.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class CorrelatedMessagePublisher : IMessagePublisher
{
    private readonly ISessionFactory _sessionFactory;
    private readonly PluginChain _pluginChain;
    private readonly CorrelationContextAccessor _contextAccessor;
    private readonly IServiceProvider _provider;
    private readonly ActivitySource _actSource = new("Messaging");

    public CorrelatedMessagePublisher(ISessionFactory sessionFactory,
        PluginChain pluginChain,
        CorrelationContextAccessor contextAccessor,
        IServiceProvider provider)
    {
        _sessionFactory = sessionFactory;
        _pluginChain = pluginChain;
        _contextAccessor = contextAccessor;
        _provider = provider;
    }

    public async Task SendMessage<TMessageType>(TMessageType message, string queue,
        Action<IMessage>? messageTransform = null)
    {
        var session = await _sessionFactory.GetSession();
        using var dest = SessionUtil.GetDestination(session, queue);
        using var producer = await session.CreateProducerAsync(dest);

        var serializer = _provider.GetRequiredService<IMessageSerializer<TMessageType>>();

        var outboundMessage = serializer.Serialize(message, session);

        using var activity = _actSource.StartActivity($"Send message to {queue}", ActivityKind.Producer,
            default(ActivityContext), new KeyValuePair<string, object?>[]
            {
                new("messaging.system", "Amqp"),
                new("messaging.destination", queue)
            });

        outboundMessage.Properties.SetString(Constants.PARENT_SPAN_ID_KEY, activity?.SpanId.ToHexString());
        outboundMessage.Properties.SetString(Constants.PARENT_TRACE_ID_KEY, activity?.TraceId.ToHexString());

        outboundMessage.NMSCorrelationID = _contextAccessor.CorrelationId;
        messageTransform?.Invoke(outboundMessage);
        var msg = await _pluginChain.HandleOutbound(outboundMessage, dest);
        Tracer.InfoFormat("sending message to {0}", queue);
        await producer.SendAsync(dest, msg);
    }
}