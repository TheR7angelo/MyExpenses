using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class System
{
    [JsonProperty("max_days_log")]
    public int MaxDaysLog { get; set; } = 15;

    [JsonProperty("max_backup_database")]
    public int MaxBackupDatabase { get; set; } = 15;

    [JsonProperty("call_back_later_time")]
    public DateTime? CallBackLaterTime { get; set; }
}