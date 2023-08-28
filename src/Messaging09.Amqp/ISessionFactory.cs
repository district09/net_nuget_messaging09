using Apache.NMS;

namespace Messaging09.Amqp;

public interface ISessionFactory
{
    Task<ISession> GetSession();
}
