using System.Threading.Tasks;

namespace Messaging09.Amqp
{
    public interface IListener
    {
        Task StartListening(string destinationName, string? selector = null);
        Task StopListening();
    }
}