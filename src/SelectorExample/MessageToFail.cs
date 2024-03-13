using System.Text.Json.Serialization;

namespace SelectorExample;

public class MessageToFail
{
  [JsonPropertyName("value")] public int Value { get; set; }
}

public class SecondMessageToFail
{
  [JsonPropertyName("value")] public int Value { get; set; }
}

public class DlqMessageToFail
{
  [JsonPropertyName("value")] public int Value { get; set; }
}
