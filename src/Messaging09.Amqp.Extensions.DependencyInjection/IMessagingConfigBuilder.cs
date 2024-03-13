using Messaging09.Amqp.Config;
using Messaging09.Amqp.Serializers;
using Messaging09.Amqp.Tracing;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.Extensions.DependencyInjection;

public delegate MessagingConfig ConfigureMessageHandling(MessagingConfig config);

public interface IMessagingConfigBuilder
{
  IServiceCollection Services { get; }

  IMessagingConfigBuilder WithListener<TMessageType, THandlerType>(string fqdn, string? selector = null)
    where TMessageType : class
    where THandlerType : MessageHandler<TMessageType>;

  IMessagingConfigBuilder WithListener<TMessageType, THandlerType, TTransformerType>(string fqdn,
    string? selector = null)
    where TMessageType : class
    where THandlerType : MessageHandler<TMessageType>
    where TTransformerType : class, IMessageSerializer<TMessageType>;

  IMessagingConfigBuilder WithSerializer<TMessageType, TSerializerType>()
    where TMessageType : class
    where TSerializerType : class, IMessageSerializer<TMessageType>;

  IMessagingConfigBuilder WithPlugin<TPluginType>()
    where TPluginType : MessagingPlugin;

  IMessagingConfigBuilder WithPublisherForType<TMessageType>() where TMessageType : class;

  IMessagingConfigBuilder WithPublisherForType<TMessageType, TSerializerType>()
    where TMessageType : class
    where TSerializerType : class, IMessageSerializer<TMessageType>;

  IMessagingConfigBuilder WithMessageHandling(ConfigureMessageHandling? configAction = null);
  IMessagingConfigBuilder WithConfig(ConfigureMessageHandling? configAction = null);

  IMessagingConfigBuilder WithTracer<TTracer>() where TTracer : ITrace, new();
  IMessagingConfigBuilder WithDotnetLogger();
}
