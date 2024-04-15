using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatimDetailedAddress
{
    [JsonProperty("shop")]
    public string? Shop { get; set; }

    [JsonProperty("house_number")]
    public string? HouseNumber { get; set; }

    [JsonProperty("road")]
    public string? Road { get; set; }

    [JsonProperty("hamlet")]
    public string? Hamlet { get; set; }

    [JsonProperty("suburb")]
    public string? Suburb { get; set; }

    [JsonProperty("city")]
    public string? City { get; set; }

    [JsonProperty("village")]
    public string? Village { get; set; }

    [JsonProperty("municipality")]
    public string? Municipality { get; set; }

    [JsonProperty("county")]
    public string? County { get; set; }

    [JsonProperty("ISO3166-2-lvl6")]
    public string? Iso31662Lvl6 { get; set; }

    [JsonProperty("state")]
    public string? State { get; set; }

    [JsonProperty("ISO3166-2-lvl4")]
    public string? Iso31662Lvl4 { get; set; }

    [JsonProperty("region")]
    public string? Region { get; set; }

    [JsonProperty("postcode")]
    public string? Postcode { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }

    [JsonProperty("country_code")]
    public string? CountryCode { get; set; }
}