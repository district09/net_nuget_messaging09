using ExampleApp.MessageHandlers;
using ExampleApp.Models;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using LogLevel = Messaging09.Amqp.Extensions.DependencyInjection.LogLevel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers();

builder.Services.AddAmqp(builder.Configuration)
    .WithListener<MessageViewModel, MyMessageHandler>("some.queue")
    .WithPublisherForType<MessageViewModel>()
    .WithDotnetLogger(LogLevel.Debug)
    .WithMessageHandling(e =>
    {
        e.DefaultAck = MessageOutcome.Failed;
        e.AddKnownException<ArgumentException>(MessageOutcome.FailedUndeliverable);
        e.SetPrefetchPolicy(1);
        return e;
    });

var app = builder.Build();

app.MapControllers();

app.Run();