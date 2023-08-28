using Apache.NMS;
using Apache.NMS.Util;
using Messaging09.Amqp.Exceptions;
using Messaging09.Amqp.Serializers;
using Tracer = Messaging09.Amqp.Tracing.Tracer;

namespace Messaging09.Amqp.Internal;

public class MessagePublisher : IMessagePublisher
{
    private readonly ISessionFactory _sessionProvider;
    private readonly PluginChain _pluginChain;
    private readonly IMessageSerializer _serializer;

    public MessagePublisher(
        ISessionFactory sessionProvider,
        PluginChain pluginChain,
        IMessageSerializer serializer)
    {
        _sessionProvider = sessionProvider;
        _pluginChain = pluginChain;
        _serializer = serializer;
    }

    public async Task SendMessage<TMessageType>(TMessageType message, string queue,
        Action<IMessage>? messageTransform = null)
    {
        var session = await _sessionProvider.GetSession();
        using var dest = SessionUtil.GetDestination(session, queue);
        using var producer = await session.CreateProducerAsync(dest);

        if (_serializer is not IMessageSerializer<TMessageType> serializer)
        {
            throw new SerializerException();
        }

        var outboundMsg = serializer.Serialize(message, await _sessionProvider.GetSession());
        messageTransform?.Invoke(outboundMsg);
        var msg = await _pluginChain.HandleOutbound(outboundMsg, dest);
        Tracer.InfoFormat("sending message to {0}", queue);
        await producer.SendAsync(dest, msg);
    }
}
