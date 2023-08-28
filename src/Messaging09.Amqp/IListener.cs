namespace Messaging09.Amqp;

public interface IListener
{
    Task StartListening(string? selector = null);
    Task StopListening();
}
