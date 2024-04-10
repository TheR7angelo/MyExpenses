using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatimAddress
{
    [JsonProperty("house_number")]
    public string? HouseNumber { get; set; }

    [JsonProperty("road")]
    public string? Road { get; set; }

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

    [JsonProperty("state")]
    public string? State { get; set; }

    [JsonProperty("region")]
    public string? Region { get; set; }

    [JsonProperty("postcode")]
    public long Postcode { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }

    [JsonProperty("country_code")]
    public string? CountryCode { get; set; }
}