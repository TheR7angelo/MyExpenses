using Newtonsoft.Json;

namespace MyExpenses.Models.IO.Smartphones;

public class AppVersionInfo
{
    [JsonProperty("version")]
    public string? VersionStr { get; set; }

    [JsonIgnore]
    public Version? Version
    {
        get =>
            string.IsNullOrEmpty(VersionStr) ?
                null
                : new Version(VersionStr);
        init => VersionStr = value?.ToString();
    }

    [JsonProperty("last_updated")]
    public DateTime? LastUpdated { get; set; }
}