using Messaging09.Amqp.Extensions.DependencyInjection;
using Messaging09.Amqp.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.SerializerExtensions;

public static class IMessagingConfigBuilderExtensions
{
  public static IMessagingConfigBuilder WithValidatedSerializer<TMessageType>(this IMessagingConfigBuilder builder)
    where TMessageType : class, IValidatedMessage<TMessageType>
  {
    builder.Services.AddScoped<IMessageSerializer<TMessageType>, ValidatingJsonSerializer<TMessageType>>();
    return builder;
  }
}
