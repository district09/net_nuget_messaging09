using Apache.NMS;
using Apache.NMS.Util;
using Messaging09.Amqp.Serializers;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class CorrelatedMessagePublisher<TMessageType> : IMessagePublisher<TMessageType>
{
    private readonly ISessionFactory _sessionFactory;
    private readonly PluginChain _pluginChain;
    private readonly IMessageSerializer<TMessageType> _serializer;
    private readonly CorrelationContextAccessor _contextAccessor;

    public CorrelatedMessagePublisher(
        ISessionFactory sessionFactory,
        PluginChain pluginChain,
        IMessageSerializer<TMessageType> serializer,
        CorrelationContextAccessor contextAccessor)
    {
        _sessionFactory = sessionFactory;
        _pluginChain = pluginChain;
        _serializer = serializer;
        _contextAccessor = contextAccessor;
    }

    public async Task SendMessage(TMessageType message, string fqdn, Action<IMessage>? messageTransform = null)
    {
        var session = await _sessionFactory.GetSession();
        using var dest = SessionUtil.GetDestination(session, fqdn);
        using var producer = await session.CreateProducerAsync(dest);

        var outboundMessage = _serializer.Serialize(message, session);
        outboundMessage.NMSCorrelationID = _contextAccessor.CorrelationId;
        messageTransform?.Invoke(outboundMessage);
        var msg = await _pluginChain.HandleOutbound(outboundMessage, dest);
        Tracer.InfoFormat("sending message to {0}", fqdn);
        await producer.SendAsync(dest, msg);
    }
}