using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using SelectorExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAmqp(builder.Configuration)
  .WithListener<MessageToFail, FailHandler>("first.fail.queue")
  .WithListener<SecondMessageToFail, SecondQueueFailHandler>("second.fail.queue")
  .WithListener<DlqMessageToFail, DlqMessageHandler>("DLQ", "_AMQ_ORIG_ADDRESS='second.fail.queue'")
  .WithPublisherForType<MessageToFail>();
var app = builder.Build();

app.MapGet("/send", async (IMessagePublisher publisher) =>
{
  for (var i = 0; i < 5; i++)
  {
    await publisher.SendMessage(new MessageToFail { Value = 1 }, "first.fail.queue");
    await publisher.SendMessage(new MessageToFail { Value = 2 }, "second.fail.queue");
  }

  return TypedResults.Ok();
});

app.Run();
