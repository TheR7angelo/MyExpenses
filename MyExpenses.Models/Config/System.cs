using Newtonsoft.Json;

namespace MyExpenses.Models.Config;

public class System
{
    [JsonProperty("max_days_log")]
    public int MaxDaysLog { get; set; } = 15;

    [JsonProperty("max_days_backup_database")]
    public int MaxDaysBackupDatabase { get; set; } = 15;
}