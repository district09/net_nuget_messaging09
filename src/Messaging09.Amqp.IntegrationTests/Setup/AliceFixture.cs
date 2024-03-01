using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging09.Amqp.IntegrationTests.Setup;

public class AliceFixture : IDisposable, IAsyncLifetime
{
  public IServiceProvider Services { get; set; }

  public MessageTracker? Messages { get; set; }

  public IHost? Host { get; set; }


  public async Task InitializeAsync()
  {
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["Name"] = "Alice",
        ["Broker:Username"] = "artemisuser",
        ["Broker:Password"] = "artemispassword",
        ["Broker:Uri"] = "amqp://localhost:5672",
        ["Broker:TimeoutSeconds"] = "10",
        ["Broker:PrefetchPolicy"] = "10"
      })
      .Build();

    var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
    builder.ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddConfiguration(configuration));
    builder.ConfigureLogging(loggingBuilder =>
    {
      loggingBuilder.ClearProviders();
      loggingBuilder.AddDebug();
    });
    BrokerTestUtils.PurgeQueue("Messaging09.Amqp.IntegrationTests.BobMessage");
    BrokerTestUtils.PurgeQueue("DLQ");

    builder.ConfigureServices(services =>
    {
      services.AddSingleton<MessageTracker>();

      services.AddAmqp(configuration)
        .WithListener<BobMessage, BobMessageHandler>("Messaging09.Amqp.IntegrationTests.BobMessage")
        .WithListener<BobMessage,BobMessageHandler>("VirtualTopic.Messaging09.Amqp.IntegrationTests.BobMessage::Consumer.Alice")
        .WithDotnetLogger()
        .WithConfig(config =>
        {
          config.AddKnownException<InvalidOperationException>(MessageOutcome.Failed);
          return config;
        });
    });

    Host = await builder.StartAsync();

    Services = Host.Services;

    Messages = Host.Services.GetRequiredService<MessageTracker>();
  }

  public Task DisposeAsync()
  {
    return Host?.StopAsync() ?? Task.CompletedTask;
  }

  public void Dispose()
  {
    Host?.Dispose();
  }
}
