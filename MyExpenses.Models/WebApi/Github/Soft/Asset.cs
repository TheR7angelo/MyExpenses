using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Github.Soft;

public class Asset
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    [JsonProperty("size")]
    public int? Size { get; set; }

    [JsonProperty("download_count")]
    public int? DownloadCount { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonProperty("browser_download_url")]
    public string? BrowserDownloadUrl { get; set; }
}