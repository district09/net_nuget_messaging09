using Messaging09.Amqp.Config;
using Messaging09.Amqp.Exceptions;
using Messaging09.Amqp.Providers;

namespace Messaging09.Amqp.Test;

public class SessionFactoryTests
{
    [Fact]
    public async Task TimingOut_ShouldThrowException()
    {
        var options = new BrokerOptions
        {
            Uri = "amqp://some_unconnectable_host:5672",
            Username = "",
            Password = "",
            PrefetchPolicy = 1,
            TimeoutSeconds = 1
        };

        var sessionFactory = new SessionFactory(options);

        var exception =
            await Assert.ThrowsAsync<BrokerConnectionException>(async () => await sessionFactory.GetSession());
        Assert.Equal(options.Uri, exception.Host);
        Assert.Equal(options.TimeoutSeconds, exception.TimeoutSeconds);
    }
}
