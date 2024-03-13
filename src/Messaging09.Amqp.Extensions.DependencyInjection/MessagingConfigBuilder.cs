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
    WithConfig(null);
  }

  public IMessagingConfigBuilder WithListener<TMessageType, THandlerType>(string fqdn, string? selector = null)
    where TMessageType : class
    where THandlerType : MessageHandler<TMessageType>
  {
    return WithListener<TMessageType, THandlerType, JsonTextMessageSerializer<TMessageType>>(fqdn, selector);
  }

  public IMessagingConfigBuilder WithListener<TMessageType, THandlerType, TSerializerType>(string fqdn,
    string? selector = null)
    where TMessageType : class
    where THandlerType : MessageHandler<TMessageType>
    where TSerializerType : class, IMessageSerializer<TMessageType>
  {
    Services.AddScoped<MessageHandler<TMessageType>, THandlerType>();

    Services.AddSingleton<IHostedService>(provider =>
    {
      var logger = provider.GetRequiredService<ILogger<ListenerHostedService<TMessageType>>>();
      var listenerFactory = provider.GetRequiredService<ListenerFactory>();
      return new ListenerHostedService<TMessageType>(logger, listenerFactory, fqdn, selector);
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

  [Obsolete("WithMessageHandling has been renamed to WithConfig for clarity")]
  public IMessagingConfigBuilder WithMessageHandling(ConfigureMessageHandling? configAction = null)
  {
    return WithConfig(configAction);
  }

  public IMessagingConfigBuilder WithConfig(ConfigureMessageHandling? configAction = null)
  {
    var handlingConfig = new MessagingConfig();
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
    Services.AddTransient<IStartupFilter, LogStartupFilter>();
    return this;
  }
}
