using Messaging09.Amqp.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class ListenerFactory
{
    private readonly IServiceProvider _provider;

    public ListenerFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IListener GetListener<TMessageType>(string queue) where TMessageType : class
    {
        return new ScopedMessageListener<TMessageType>(
            _provider.GetRequiredService<ILogger<ScopedMessageListener<TMessageType>>>(),
            _provider.GetRequiredService<IServiceScopeFactory>(),
            _provider.GetRequiredService<ISessionFactory>(),
            _provider.GetRequiredService<MessagingConfig>(), queue);
    }
}