using ExampleApp.Models;
using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;

namespace ExampleApp.MessageHandlers;

public class MyMessageHandler : MessageHandler<MessageViewModel>
{
    private readonly ILogger<MyMessageHandler> _logger;

    public MyMessageHandler(IMessageSerializer<MessageViewModel> serializer, ILogger<MyMessageHandler> logger) :
        base(serializer)
    {
        _logger = logger;
    }

    protected override async Task<MessageOutcome> Handle(MessageEnvelope<MessageViewModel> envelope)
    {
        _logger.LogInformation("message received with count {Count}, correlation: {Correlation}",
            envelope.Message.Count, envelope.Original.NMSCorrelationID);
        //await Task.Delay(1000);
        return MessageOutcome.Ack;
    }
}
