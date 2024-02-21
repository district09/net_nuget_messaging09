using FluentValidation;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Messaging09.Amqp.SerializerExtensions.FluentValidation;
using ValidationExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAmqp(builder.Configuration)
  .WithListener<ValidatedMessage, Handler>("validated.message.queue")
  .WithPublisherForType<ValidatedMessage>()
  .WithValidatedSerializer<ValidatedMessage>()
  .WithDotnetLogger()
  .WithConfig(config =>
  {
    config.AddKnownException<ValidationException>(MessageOutcome.Reject);
    return config;
  });

var app = builder.Build();

app.MapGet("/{msg}", async (IMessagePublisher publisher, string msg) =>
{
  await publisher.SendMessage(new ValidatedMessage() { Hello = msg }, "validated.message.queue");
  return "OK";
});

app.Run();
