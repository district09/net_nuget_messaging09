using System.Text.Json.Serialization;

namespace NoDIWorkerExample;

public class PingMessage
{
    [JsonPropertyName("ping")]
    public string Ping { get; set; }
}