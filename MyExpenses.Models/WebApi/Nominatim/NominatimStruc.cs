using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatimStruc
{
    [JsonProperty("place_id")]
    public long? PlaceId { get; set; }

    [JsonProperty("licence")]
    public string? Licence { get; set; }

    [JsonProperty("osm_type")]
    public string? OsmType { get; set; }

    [JsonProperty("osm_id")]
    public long? OsmId { get; set; }

    [JsonProperty("lat")]
    public float? Latitude { get; set; }

    [JsonProperty("lon")]
    public float? Longitude { get; set; }

    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonProperty("address")]
    public NominatimAddress? Address { get; set; }

    [JsonProperty("boundingbox")]
    public IEnumerable<float>? BoundingBox { get; set; }
}