using System.Runtime.Serialization;

namespace Messaging09.Amqp.Test.TestClasses;

[Serializable]
public class TestException : Exception
{
    public TestException()
    {
    }

    protected TestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TestException(string? message) : base(message)
    {
    }

    public TestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}