using Newtonsoft.Json;

namespace MyExpenses.Models.Config.Interfaces;

public class Clock
{
    [JsonProperty("is_24_hours")]
    public bool Is24Hours { get; set; } = true;
}