using Messaging09.Amqp.Config;
using Messaging09.Amqp.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static MessagingConfigBuilder AddAmqp(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BrokerOptions>(configuration.GetSection(BrokerOptions.Prefix));
        services.AddSingleton<BrokerOptions>((p) =>
            configuration.GetSection(BrokerOptions.Prefix).Get<BrokerOptions>());
        services.AddSingleton<ISessionFactory, SessionFactory>();
        services.AddTransient<PluginChain>(provider =>
        {
            var plugins = provider.GetRequiredService<IEnumerable<MessagingPlugin>>();

            return new PluginChain(plugins, provider.GetRequiredService<MessagingConfig>());
        });
        services.AddScoped<CorrelationContextAccessor>();

        services.AddSingleton<ListenerFactory>(provider => new ListenerFactory(provider));

        return new MessagingConfigBuilder(services);
    }
}
