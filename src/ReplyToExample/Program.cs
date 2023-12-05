using Apache.NMS.AMQP;
using Messaging09.Amqp;
using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using ReplyToExample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAmqp(builder.Configuration)
  .WithListener<MessageDto, MessageHandler>("some.intake.queue")
  .WithPublisherForType<MessageDto>()
  .WithDotnetLogger();

var app = builder.Build();


app.MapGet("/", async (HttpContext p) =>
{
  var publisher = p.RequestServices.GetRequiredService<IMessagePublisher>();

  foreach (var replyTopic in new[]
           {
             (0, "VirtualTopic.some.reply.topic::Consumer.first"),
             (1, "VirtualTopic.some.reply.topic::Consumer.second"),
             (2, "VirtualTopic.some.reply.topic::Consumer.third")
           })
  {
    var dto = new MessageDto() { Count = replyTopic.Item1 };

    // with reply queue
    await publisher.SendMessage(dto, "some.intake.queue",
      message => message.NMSReplyTo = new NmsQueue(replyTopic.Item2));
  }

  // default fallback
  await publisher.SendMessage(new MessageDto() { Count = 3 }, "some.intake.queue");

  return "SENT";
});

app.Run();
