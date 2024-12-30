using MyExpenses.Models.Config.Interfaces;
using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class Configuration
{
    public delegate void ConfigurationChangedEventHandler();

    public static event ConfigurationChangedEventHandler? ConfigurationChanged;

    public static void OnConfigurationChanged()
    {
        ConfigurationChanged?.Invoke();
    }

    [JsonProperty("system")]
    public System System { get; set; } = new();

    [JsonProperty("interface")]
    public Interface Interface { get; set; } = new();

    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
}