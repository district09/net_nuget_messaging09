using System.Xml;
using System.Xml.Serialization;
using Messaging09.Amqp.Exceptions;

namespace Messaging09.Amqp.Serializers.Text;

public class XmlTextMessageSerializer<TDataType> : TextMessageSerializer<TDataType> where TDataType : class
{
    protected override TDataType DeserializeFromText(string text)
    {
        using var stream = GenerateStreamFromString(text);
        var serializer = new XmlSerializer(typeof(TDataType));
        if (serializer.Deserialize(stream) is not TDataType data)
        {
            throw new MessageFormatException(typeof(XmlTextMessageSerializer<TDataType>), typeof(TDataType),
                typeof(object));
        }

        return data;
    }

    protected override string SerializeToText(TDataType message)
    {
        var serializer = new XmlSerializer(typeof(TDataType));

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter);

        serializer.Serialize(xmlWriter, message);
        return stringWriter.ToString();
    }

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
