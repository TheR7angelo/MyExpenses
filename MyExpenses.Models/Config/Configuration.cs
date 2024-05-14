using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class Configuration
{
    [JsonProperty("log")]
    public Log? Log { get; set; }
}