﻿using Messaging09.Amqp.Extensions.DependencyInjection;
using Messaging09.Amqp.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging09.Amqp.SerializerExtensions.FluentValidation;

public static class MessagingConfigBuilderExtensions
{
  public static IMessagingConfigBuilder WithValidatedSerializer<TMessageType>(this IMessagingConfigBuilder builder)
    where TMessageType : class, IValidatedMessage<TMessageType>
  {
    builder.Services.AddScoped<IMessageSerializer<TMessageType>, ValidatingJsonSerializer<TMessageType>>();
    return builder;
  }
}
