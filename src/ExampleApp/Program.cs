using ExampleApp.MessageHandlers;
using ExampleApp.Models;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAmqp(builder.Configuration)
    .WithListener<MessageViewModel, MyMessageHandler>("VirtualTopic.some.topic::Consumer.VirtualTopic.some.topic")
    .WithPublisherForType<MessageViewModel>()
    .WithDotnetLogger()
    .WithConfig(e =>
    {
        e.DefaultAck = MessageOutcome.Failed;
        e.AddKnownException<ArgumentException>(MessageOutcome.FailedUndeliverable);
        return e;
    });

var app = builder.Build();

app.MapControllers();

app.Run();
