using System.Threading.Tasks;
using Apache.NMS;

namespace Messaging09.Amqp
{
    public interface IConnectionFactory
    {
        public Task<IConnection> GetConnection();
    }
}