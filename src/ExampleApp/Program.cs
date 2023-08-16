using ExampleApp.MessageHandlers;
using ExampleApp.Models;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Messaging09.Amqp.Tracing;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAmqp(builder.Configuration)
    .WithPlugin<TracingPlugin>()
    .WithListener<MessageViewModel, MyMessageHandler>("VirtualTopic.some.topic::Consumer.VirtualTopic.some.topic")
    .WithPublisherForType<MessageViewModel>()
    .WithDotnetLogger()
    .WithMessageHandling(e =>
    {
        e.DefaultAck = MessageOutcome.Failed;
        e.AddKnownException<ArgumentException>(MessageOutcome.FailedUndeliverable);
        return e;
    });

var app = builder.Build();

app.MapControllers();

app.Run();