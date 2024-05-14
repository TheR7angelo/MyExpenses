using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class Log
{
    [JsonProperty("max_days_log")]
    public int MaxDaysLog { get; set; } = 15;
}