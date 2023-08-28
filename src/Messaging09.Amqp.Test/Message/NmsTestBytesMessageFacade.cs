using Apache.NMS;
using Apache.NMS.AMQP.Message.Facade;

namespace Messaging09.Amqp.Test.Message;

public class NmsTestBytesMessageFacade : NmsTestMessageFacade, INmsBytesMessageFacade
{
    private BinaryWriter bytesOut = null;
    private BinaryReader bytesIn = null;
    private byte[] content = null;

    public NmsTestBytesMessageFacade()
    {
        content = new byte[0];
    }

    public NmsTestBytesMessageFacade(byte[] content)
    {
        this.content = content;
    }

    public BinaryReader GetDataReader()
    {
        if (bytesOut != null)
        {
            throw new IllegalStateException("Body is being written to, cannot perform a read.");
        }

        return bytesIn ?? (bytesIn = new BinaryReader(new MemoryStream(content)));
    }

    public BinaryWriter GetDataWriter()
    {
        if (bytesIn != null)
        {
            throw new IllegalStateException("Body is being read from, cannot perform a write.");
        }

        return bytesOut ?? (bytesOut = new BinaryWriter(new MemoryStream()));
    }

    public void Reset()
    {
        if (bytesOut != null)
        {
            MemoryStream byteStream = new MemoryStream((int)bytesOut.BaseStream.Length);
            bytesOut.BaseStream.Position = 0;
            bytesOut.BaseStream.CopyTo(byteStream);

            content = byteStream.ToArray();

            byteStream.Close();
            bytesOut.Close();
            bytesOut = null;
        }
        else if (bytesIn != null)
        {
            bytesIn.Close();
            bytesIn = null;
        }
    }

    public override void ClearBody()
    {
        this.Reset();
        content = new byte[0];
    }

    public long BodyLength => content?.LongLength ?? 0;
}
