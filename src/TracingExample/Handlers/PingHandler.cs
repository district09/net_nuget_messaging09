using Messaging09.Amqp;
using Messaging09.Amqp.Serializers;
using TracingExample.Viewmodels;

namespace TracingExample.Handlers;

public class PingHandler : MessageHandler<PingMessage>
{
    private readonly IMessagePublisher _publisher;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<PingHandler> _logger;

    public PingHandler(IMessageSerializer<PingMessage> serializer, IMessagePublisher publisher,
        IHttpClientFactory clientFactory, ILogger<PingHandler> logger) : base(serializer)
    {
        _publisher = publisher;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    protected override async Task<MessageOutcome> Handle(MessageEnvelope<PingMessage> envelope)
    {
        _logger.LogInformation("received pingmessage with count {Count}", envelope.Message.PingCount);
        if (envelope.Message.PingCount >= 10) return MessageOutcome.Ack;

        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://example.org"));
        _logger.LogInformation("Received a {Status} status code from http call", response.StatusCode);
        await Task.Delay(500);
        await _publisher.SendMessage(new PongMessage() { PingCount = envelope.Message.PingCount + 1 },
            "VirtualTopic.some.topic");

        return MessageOutcome.Ack;
    }
}
