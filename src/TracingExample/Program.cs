using Elastic.Apm.NetCoreAll;
using Elastic.Extensions.Logging;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Messaging09.Amqp.Serializers.Text;
using Messaging09.Amqp.Tracing;
using TracingExample.Handlers;
using TracingExample.Viewmodels;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAllElasticApm();

builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddElasticsearch(); });

builder.Services.AddAmqp(builder.Configuration)
  .WithListener<PingMessage, PingHandler>("ping.queue")
  .WithListener<PongMessage, PongHandler>("VirtualTopic.some.topic::Consumer.VirtualTopic.some.topic")
  .WithDotnetLogger()
  .WithPublisherForType<PingMessage>()
  .WithPublisherForType<PongMessage>()
  .WithConfig(e =>
  {
    e.AddKnownException<ArgumentException>(MessageOutcome.FailedUndeliverable);
    e.AddKnownException<ArgumentOutOfRangeException>(MessageOutcome.Reject);
    e.UnhandledExceptionAck = MessageOutcome.Reject;
    return e;
  });
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/",
    async (IMessagePublisher publisher) =>
    {
        await publisher.SendMessage(new PingMessage() { PingCount = 0 }, "ping.queue");
        return Results.Ok();
    });

app.Run();
