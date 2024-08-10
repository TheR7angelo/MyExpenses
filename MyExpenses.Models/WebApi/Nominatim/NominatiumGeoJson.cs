using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatiumGeoJson
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("coordinates")]
    public List<List<List<double>>>? Coordinates { get; set; }
}