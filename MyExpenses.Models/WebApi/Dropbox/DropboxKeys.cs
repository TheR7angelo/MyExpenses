using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.DropBox;

public class DropboxKeys
{
    [JsonProperty("app_key")]
    public string? AppKey { get; set; }

    [JsonProperty("app_secret")]
    public string? AppSecret { get; set; }

    [JsonProperty("redirect_uri")]
    public string? RedirectUri { get; set; }

}