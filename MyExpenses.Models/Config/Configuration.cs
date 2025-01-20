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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Intentionally initializing the property with a default object to ensure a valid instance
    // even if the JSON does not provide a corresponding value.
    [JsonProperty("system")]
    public System System { get; set; } = new();

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Intentionally initializing the property with a default object to ensure a valid instance
    // even if the JSON does not provide a corresponding value.
    [JsonProperty("interface")]
    public Interface Interface { get; set; } = new();

    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
}