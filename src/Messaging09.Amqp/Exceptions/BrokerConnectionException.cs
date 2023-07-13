using System.Runtime.Serialization;

namespace Messaging09.Amqp.Exceptions
{
    [Serializable]
    public class BrokerConnectionException : Exception
    {
        private readonly string? _host;
        private readonly int _seconds;

        protected BrokerConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _host = info.GetString("host");
            _seconds = info.GetInt32("timeoutSeconds");
        }

        public BrokerConnectionException(string host, int seconds) : base(
            $"Could not connect to {host} in {seconds} seconds")
        {
            _host = host;
            _seconds = seconds;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("host", _host);
            info.AddValue("timeoutSeconds", _seconds);
            base.GetObjectData(info, context);
        }
    }
}