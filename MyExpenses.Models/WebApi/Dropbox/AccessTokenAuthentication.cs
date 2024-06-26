using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.DropBox;

public class AccessTokenAuthentication
{
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string? TokenType { get; set; }

    [JsonProperty("expires_in")]
    public int? ExpiresIn { get; set; }

    [JsonProperty("scope")]
    public string? Scope { get; set; }

    [JsonProperty("uid")]
    public string? Uid { get; set; }

    [JsonProperty("account_id")]
    public string? AccountId { get; set; }
}