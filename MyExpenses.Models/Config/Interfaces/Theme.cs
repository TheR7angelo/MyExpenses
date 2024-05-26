using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Theme
{
    [JsonProperty("base_theme")]
    public string BaseTheme { get; set; } = "Inherit";

    [JsonProperty("hexadecimal_code_primary_color")]
    public string HexadecimalCodePrimaryColor { get; set; } = "#FF008000";

    [JsonProperty("hexadecimal_code_secondary_color")]
    public string HexadecimalCodeSecondaryColor { get; set; } = "#FFFFA500";
}