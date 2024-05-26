using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Interface
{
    [JsonProperty("theme")]
    public Theme Theme { get; set; } = new();

    [JsonProperty("clock")]
    public Clock Clock { get; set; } = new();
}