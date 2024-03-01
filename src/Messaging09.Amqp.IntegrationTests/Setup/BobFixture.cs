using Messaging09.Amqp.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Messaging09.Amqp.IntegrationTests.Setup;

public class BobFixture : IDisposable, IAsyncLifetime
{
  public IServiceProvider Services { get; set; }

  public IServiceScopeFactory ScopeFactory { get; set; }

  public MessageTracker? Messages { get; set; }

  public IHost? Host { get; set; }

  public async Task InitializeAsync()
  {
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["Name"] = "Bob",
        ["Broker:Username"] = "artemisuser",
        ["Broker:Password"] = "artemispassword",
        ["Broker:Uri"] = "amqp://localhost:5672",
        ["Broker:TimeoutSeconds"] = "10",
        ["Broker:PrefetchPolicy"] = "10"
      })
      .AddUserSecrets(GetType().Assembly)
      .Build();

    var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();

    builder.ConfigureServices(services =>
    {
      services.AddSingleton<MessageTracker>();
      services.AddAmqp(configuration)
        .WithPublisherForType<BobMessage>();
    });

    Host = await builder.StartAsync();

    Services = Host.Services;

    Messages = Host.Services.GetRequiredService<MessageTracker>();

    ScopeFactory = Services.GetRequiredService<IServiceScopeFactory>();
  }

  public async Task SendMessage(BobMessage msg, string queue)
  {
    using var scope = ScopeFactory.CreateScope();
    var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
    await publisher.SendMessage(msg, queue);
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
