using Newtonsoft.Json;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatimSearchResult
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

    [JsonProperty("class")]
    public string? Class { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("place_rank")]
    public int? PlaceRank { get; set; }

    [JsonProperty("importance")]
    public double? Importance { get; set; }

    [JsonProperty("addresstype")]
    public string? AddressType { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonProperty("address")]
    public NominatimDetailedAddress? Address { get; set; }

    [JsonProperty("boundingbox")]
    public IEnumerable<float>? BoundingBox { get; set; }

    public override string? ToString()
        => DisplayName;
}