using System.Text.Json.Serialization;
using Messaging09.Amqp.Serializers.Text;
using Messaging09.Amqp.Test.Message;

namespace Messaging09.Amqp.Test.Serializers;

public class JsonTextMessageSerializerTests
{
    private readonly TestMessageFactory _messageFactory = new();

    private class TestMessage
    {
        public TestMessage(int count)
        {
            Count = count;
        }

        [JsonPropertyName("count")]
        public int Count { get; init; }
    }

    [Theory]
    [InlineData("{\"count\":1}", 1)]
    [InlineData("{\"count\":10}", 10)]
    [InlineData("{\"count\":-10}", -10)]
    [InlineData("{\"count\":1, \"hello\":\"world\"}", 1)]
    public void Deserialize_ShouldReturnCorrectData(string jsonStr, int expected)
    {
        var message = _messageFactory.CreateTextMessage(jsonStr);
        var serializer = new JsonTextMessageSerializer<TestMessage>();
        var msg = serializer.Deserialize(message);

        Assert.Equal(expected, msg.Count);
    }
}
