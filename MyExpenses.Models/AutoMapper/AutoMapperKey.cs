using Newtonsoft.Json;

namespace MyExpenses.Models.AutoMapper;

public class AutoMapperKey
{
    [JsonProperty("license_key")]
    public required string LicenceKey { get; init; }

    [JsonProperty("valid_until")]
    public required DateOnly ValidUntil { get; init; }
}