using System.Text.Json.Serialization;

namespace ExampleApp.Models;

public class MessageViewModel
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}