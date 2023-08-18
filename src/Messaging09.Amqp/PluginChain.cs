using Apache.NMS;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.Plugins;

namespace Messaging09.Amqp;

public class PluginChain
{
    private readonly MessagingPlugin _first;

    public PluginChain(IEnumerable<MessagingPlugin> plugins, MessagingConfig config)
    {
        _first = new ErrorHandlingPlugin(config);
        foreach (var plugin in plugins)
        {
            _first.SetNext(plugin);
        }
    }

    public async Task<MessageOutcome> HandleInbound<TMessageType>(IMessage message,
        MessageHandler<TMessageType> handler)
        where TMessageType : class
    {
        return await _first.HandleInbound(message, handler);
    }

    public async Task<IMessage> HandleOutbound(IMessage msg, IDestination fqdn)
    {
        return await _first.HandleOutbound(msg, fqdn);
    }
}