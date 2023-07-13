using System;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.Util;
using Messaging09.Amqp.Serializers;
using Tracer = Messaging09.Amqp.Tracing.Tracer;

namespace Messaging09.Amqp.Internal;

public class MessagePublisher<TMessageType> : IMessagePublisher<TMessageType>
{
    private readonly ISessionFactory _sessionProvider;
    private readonly PluginChain _pluginChain;
    private readonly IMessageSerializer<TMessageType> _serializer;

    public MessagePublisher(
        ISessionFactory sessionProvider, 
        PluginChain pluginChain,
        IMessageSerializer<TMessageType> serializer)
    {
        _sessionProvider = sessionProvider;
        _pluginChain = pluginChain;
        _serializer = serializer;
    }

    public async Task SendMessage(
        TMessageType message,
        string fqdn,
        Action<IMessage>? messageTransform = null)
    {
        var session = await _sessionProvider.GetSession();
        using var dest = SessionUtil.GetDestination(session, fqdn);
        using var producer = await session.CreateProducerAsync(dest);

        var outboundMsg = _serializer.Serialize(message, await _sessionProvider.GetSession());
        messageTransform?.Invoke(outboundMsg);
        var msg = await _pluginChain.HandleOutbound(outboundMsg, dest);
        Tracer.InfoFormat("sending message to {0}", fqdn);
        await producer.SendAsync(dest, msg);
    }
}