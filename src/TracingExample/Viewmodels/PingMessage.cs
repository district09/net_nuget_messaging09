using System.Text.Json.Serialization;

namespace TracingExample.Viewmodels;

public class PingMessage
{
    [JsonPropertyName("ping_count")]
    public int PingCount { get; set; }
}
