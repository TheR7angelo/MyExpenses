using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Github.Soft;

public class Release
{
    [JsonProperty("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonProperty("tag_name")]
    public string? TagName { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("prerelease")]
    public bool? Prerelease { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonProperty("assets")]
    public List<Asset?>? Assets { get; set; }

    [JsonProperty("body")]
    public string? Body { get; set; }
}