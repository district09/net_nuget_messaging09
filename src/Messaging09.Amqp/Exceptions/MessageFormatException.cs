using System.Runtime.Serialization;

namespace Messaging09.Amqp.Exceptions;

public class MessageFormatException : Exception
{
    public MessageFormatException()
    {
    }

    public MessageFormatException(string? message) : base(message)
    {
    }

    public MessageFormatException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public MessageFormatException(Type serializer, Type requiredFormat, Type foundFormat) : base(
        $"{serializer.FullName} expects {requiredFormat.FullName}, but received {foundFormat.FullName}")
    {
    }

    protected MessageFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
