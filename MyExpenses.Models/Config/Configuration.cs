using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class Configuration
{
    [JsonProperty("log")]
    public Log Log { get; set; } = new();

    [JsonProperty("interface")]
    public Interface Interface { get; set; } = new();

    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
}