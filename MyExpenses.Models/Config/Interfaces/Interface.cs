using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Interface
{
    [JsonProperty("theme")]
    public Theme Theme { get; set; } = new();

    [JsonProperty("language")]
    public string? Language { get; set; } = "en-001";

    [JsonProperty("clock")]
    public Clock Clock { get; set; } = new();
}