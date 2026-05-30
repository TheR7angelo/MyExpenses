using System.Text.Json.Serialization;

namespace MyExpenses.WebApi.Entities;

public class NominatimSearchResult
{
    [JsonPropertyName("place_id")]
    public long? PlaceId { get; set; }

    [JsonPropertyName("licence")]
    public string? Licence { get; set; }

    [JsonPropertyName("osm_type")]
    public string? OsmType { get; set; }

    [JsonPropertyName("osm_id")]
    public long? OsmId { get; set; }

    [JsonPropertyName("lat")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double? Latitude { get; set; }

    [JsonPropertyName("lon")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double? Longitude { get; set; }

    [JsonPropertyName("class")]
    public string? Class { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("place_rank")]
    public int? PlaceRank { get; set; }

    [JsonPropertyName("importance")]
    public double? Importance { get; set; }

    [JsonPropertyName("addresstype")]
    public string? AddressType { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("address")]
    public NominatimDetailedAddress? Address { get; set; }

    [JsonPropertyName("boundingbox")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public IEnumerable<float>? BoundingBox { get; set; }

    [JsonPropertyName("geojson")]
    public NominatimGeoJson? GeoJson { get; set; }

    public override string? ToString()
        => DisplayName;
}