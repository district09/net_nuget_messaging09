using Messaging09.Amqp.Config;
using Messaging09.Amqp.Serializers;
using Messaging09.Amqp.Serializers.Text;
using Messaging09.Amqp.Tracing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public class MessagingConfigBuilder : IMessagingConfigBuilder
{
    public IServiceCollection Services { get; }

    public MessagingConfigBuilder(IServiceCollection services)
    {
        Services = services;
        WithMessageHandling(null);
    }

    public IMessagingConfigBuilder WithListener<TMessageType, THandlerType>(string fqdn)
        where TMessageType : class
        where THandlerType : MessageHandler<TMessageType>
    {
        return WithListener<TMessageType, THandlerType, JsonTextMessageSerializer<TMessageType>>(fqdn);
    }

    public IMessagingConfigBuilder WithListener<TMessageType, THandlerType, TSerializerType>(string fqdn)
        where TMessageType : class
        where THandlerType : MessageHandler<TMessageType>
        where TSerializerType : class, IMessageSerializer<TMessageType>
    {
        Services.AddScoped<MessageHandler<TMessageType>, THandlerType>();

        Services.AddSingleton<IHostedService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<ListenerHostedService<TMessageType>>>();
            var listenerFactory = provider.GetRequiredService<ListenerFactory>();
            return new ListenerHostedService<TMessageType>(logger, listenerFactory, fqdn);
        });
        return WithSerializer<TMessageType, TSerializerType>();
    }

    public IMessagingConfigBuilder WithSerializer<TMessageType, TSerializerType>()
        where TMessageType : class
        where TSerializerType : class, IMessageSerializer<TMessageType>
    {
        Services.AddScoped<IMessageSerializer<TMessageType>, TSerializerType>();
        return this;
    }

    public IMessagingConfigBuilder WithPlugin<TPluginType>()
        where TPluginType : MessagingPlugin
    {
        Services.AddTransient<MessagingPlugin, TPluginType>();
        return this;
    }

    public IMessagingConfigBuilder WithPublisherForType<TMessageType>() where TMessageType : class
    {
        return WithPublisherForType<TMessageType, JsonTextMessageSerializer<TMessageType>>();
    }

    public IMessagingConfigBuilder WithPublisherForType<TMessageType, TSerializerType>()
        where TMessageType : class
        where TSerializerType : class, IMessageSerializer<TMessageType>
    {
        Services.AddScoped<IMessagePublisher, CorrelatedMessagePublisher>();
        return WithSerializer<TMessageType, TSerializerType>();
    }

    public IMessagingConfigBuilder WithMessageHandling(ConfigureMessageHandling? configAction = null)
    {
        var handlingConfig = new MessageHandlingConfig();
        var config = configAction?.Invoke(handlingConfig);
        Services.Replace(ServiceDescriptor.Singleton(config ?? handlingConfig));
        return this;
    }

    public IMessagingConfigBuilder WithTracer<TTracer>() where TTracer : ITrace, new()
    {
        Tracer.Trace = new TTracer();
        return this;
    }

    public IMessagingConfigBuilder WithDotnetLogger()
    {
        // var provider = Services.BuildServiceProvider();
        Services.AddTransient<IStartupFilter, LogStartupFilter>();
        //Services.AddHostedService<LogHostedService>(e => new LogHostedService(e.GetRequiredService<ILogger<ITrace>>()));
        // var logger = provider.GetRequiredService<ILoggerFactory>();
        // var x = logger.CreateLogger(typeof(Tracer).FullName!);
        // Tracer.Trace = new LogTracer(x);
        return this;
    }
}