using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Github.Soft;

public class Release
{
    [JsonProperty("html_url")]
    public string? HtmlUrl { get; set; }

    [JsonProperty("tag_name")]
    public string? TagName { get; set; }

    [JsonIgnore]
    public Version? Version
    {
        get
        {
            if (string.IsNullOrEmpty(TagName)) return null;
            var tagName = TagName.Replace("v", string.Empty);

            // This implementation avoids unnecessary allocations by only creating
            // a Version instance when TagName is not null or empty. The use of
            // Replace ensures that any leading 'v' is removed before parsing,
            // keeping the code simple and efficient while adhering to expected input formats.
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new Version(tagName);
        }
    }

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