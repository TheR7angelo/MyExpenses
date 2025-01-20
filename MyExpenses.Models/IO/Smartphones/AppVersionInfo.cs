using Newtonsoft.Json;

namespace MyExpenses.Models.IO.Smartphones;

public class AppVersionInfo
{
    [JsonProperty("version")]
    public string? VersionStr { get; set; }

    // The Version object is created lazily and only if VersionStr contains valid data.
    // This avoids unnecessary allocations while ensuring the property returns a valid object when needed.
    // ReSharper disable once HeapView.ObjectAllocation.Evident
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