using System.Text.Json;

namespace Messaging09.Amqp.Serializers.Text;

public class JsonTextMessageSerializer<TMessageType> : TextMessageSerializer<TMessageType>
{
    protected override TMessageType DeserializeFromText(string text)
    {
        return JsonSerializer.Deserialize<TMessageType>(text);
    }

    protected override string SerializeToText(TMessageType message)
    {
        return JsonSerializer.Serialize(message);
    }
}