using Messaging09.Amqp;
using Messaging09.Amqp.Config;
using Messaging09.Amqp.Internal;
using Messaging09.Amqp.Plugins;
using Messaging09.Amqp.Providers;
using Messaging09.Amqp.Serializers.Text;

namespace NoDIWorkerExample;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var brokerOptions = new BrokerOptions
        {
            Uri = "amqp://localhost:5672",
            Username = "artemisuser",
            Password = "artemispassword",
            TimeoutSeconds = 20,
            PrefetchPolicy = 1
        };

        var messageHandlingConfig = new MessagingConfig
        {
            DefaultAck = MessageOutcome.Failed,
            UnhandledExceptionAck = MessageOutcome.FailedUndeliverable
        };
        messageHandlingConfig.AddKnownException<HttpRequestException>(MessageOutcome.Failed);

        var sessionFactory = new SessionFactory(brokerOptions);

        var handler = new CustomHandler(new JsonTextMessageSerializer<PingMessage>());

        var listener = new MessageListener<PingMessage>(sessionFactory, messageHandlingConfig,
            new PluginChain(Array.Empty<MessagingPlugin>(), messageHandlingConfig), handler, "some.queue");

        var publisher = new MessagePublisher(sessionFactory,
            new PluginChain(Array.Empty<MessagingPlugin>(), messageHandlingConfig),
            new JsonTextMessageSerializer<PingMessage>());

        await listener.StartListening();
    }
}
