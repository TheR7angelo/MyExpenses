using System.Text.Json.Serialization;

namespace MyExpenses.WebApi.Entities;

public class NominatimDetailedAddress
{
    [JsonPropertyName("amenity")]
    public string? Amenity { get; set; }

    [JsonPropertyName("shop")]
    public string? Shop { get; set; }

    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    [JsonPropertyName("road")]
    public string? Road { get; set; }

    [JsonPropertyName("residential")]
    public string? Residential { get; set; }

    [JsonPropertyName("hamlet")]
    public string? Hamlet { get; set; }

    [JsonPropertyName("suburb")]
    public string? Suburb { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("village")]
    public string? Village { get; set; }

    [JsonPropertyName("municipality")]
    public string? Municipality { get; set; }

    [JsonPropertyName("county")]
    public string? County { get; set; }

    [JsonPropertyName("ISO3166-2-lvl6")]
    public string? Iso31662Lvl6 { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("ISO3166-2-lvl4")]
    public string? Iso31662Lvl4 { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
}