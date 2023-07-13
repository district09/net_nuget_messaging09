using Amqp.Types;
using Apache.NMS;
using Apache.NMS.AMQP.Message.Facade;

namespace Messaging09.Amqp.Test.Message;

public class NmsTestStreamMessageFacade : NmsTestMessageFacade, INmsStreamMessageFacade
{
    private readonly List stream = new();
    private int index = -1;

    public object Peek()
    {
        if (stream.Count == 0 || index + 1 >= stream.Count)
            throw new MessageEOFException("Attempted to read past the end of the stream");

        return stream[index + 1];
    }

    public void Pop()
    {
        if (stream.Count == 0 || index + 1 >= stream.Count)
            throw new MessageEOFException("Attempted to read past the end of the stream");

        index++;
    }

    public void Reset()
    {
        index = -1;
    }

    public void Put(object value)
    {
        stream.Add(value);
    }

    public override void ClearBody()
    {
        stream.Clear();
        index = -1;
    }
}