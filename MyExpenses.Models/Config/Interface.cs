using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class Interface
{
    [JsonProperty("base_theme")]
    public string BaseTheme { get; set; } = "Inherit";

    [JsonProperty("primary_color")]
    public string PrimaryColor { get; set; } = "Green";

    [JsonProperty("secondary_color")]
    public string SecondaryColor { get; set; } = "Orange";
}