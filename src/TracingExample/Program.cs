using Elastic.Apm.NetCoreAll;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Messaging09.Amqp.Tracing;
using TracingExample.Handlers;
using TracingExample.Viewmodels;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseAllElasticApm();

builder.Services.AddAmqp(builder.Configuration)
    .WithPlugin<TracingPlugin>()
    .WithListener<PingMessage, PingHandler>("ping.queue")
    .WithListener<PongMessage, PongHandler>("pong.queue")
    .WithDotnetLogger()
    .WithPublisherForType<PingMessage>()
    .WithPublisherForType<PongMessage>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/",
    async (IMessagePublisher publisher) =>
    {
        await publisher.SendMessage(new PingMessage() { PingCount = 0 }, "ping.queue");
        return Results.Ok();
    });

app.Run();