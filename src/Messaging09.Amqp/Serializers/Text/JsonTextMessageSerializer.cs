using System.Text.Json;
using Messaging09.Amqp.Tracing;

namespace Messaging09.Amqp.Serializers.Text;

public class JsonTextMessageSerializer<TMessageType> : TextMessageSerializer<TMessageType>
{
    protected override TMessageType DeserializeFromText(string text)
    {
        var msg = JsonSerializer.Deserialize<TMessageType>(text);
        Tracer.Debug("successfully deserialized message");
        return msg;
    }

    protected override string SerializeToText(TMessageType message)
    {
        return JsonSerializer.Serialize(message);
    }
}