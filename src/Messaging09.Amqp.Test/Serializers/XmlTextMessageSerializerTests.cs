using System.Xml.Serialization;
using Messaging09.Amqp.Serializers.Text;
using Messaging09.Amqp.Test.Message;

namespace Messaging09.Amqp.Test.Serializers;

public class XmlTextMessageSerializerTests
{
    private readonly TestMessageFactory _messageFactory = new();

    [XmlRoot("Test")]
    public class TestMessage
    {
        [XmlElement("Count")]
        public int Count { get; set; }
    }

    [Theory]
    [InlineData("Serializers/xmlData/1.xml", 1)]
    [InlineData("Serializers/xmlData/no_declaration.xml", 1)]
    public void Deserialize_ShouldReturnCorrectData(string filename, int expected)
    {
        var fileContent = File.ReadAllText(filename);

        var message = _messageFactory.CreateTextMessage(fileContent);
        var serializer = new XmlTextMessageSerializer<TestMessage>();
        var msg = serializer.Deserialize(message);

        Assert.Equal(expected, msg.Count);
    }
}
