namespace Messaging09.Amqp.Config;

public class BrokerOptions
{
    public const string Prefix = "Broker";
    public string Uri { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int TimeoutSeconds { get; set; }
    public int PrefetchPolicy { get; set; } = 2;
}
