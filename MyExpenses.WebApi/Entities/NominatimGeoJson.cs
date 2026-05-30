using System.Text.Json.Serialization;

namespace MyExpenses.WebApi.Entities;

public class NominatimGeoJson
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("coordinates")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public IEnumerable<float>? Coordinates { get; set; }

    // public List<List<double>>? GetLineStringCoordinates()
    // {
    //     if (Type?.ToLower() is "linestring" && Coordinates is not null)
    //     {
    //         return Coordinates.ToObject<List<List<double>>>()!;
    //     }
    //
    //     if (Coordinates is null) return null;
    //     throw new InvalidOperationException("Coordinates doesn't represent a LineString");
    // }
    //
    // public List<List<List<double>>>? GetPolygonCoordinates()
    // {
    //     if (Type?.ToLower() is "polygon" && Coordinates is not null)
    //     {
    //         return Coordinates.ToObject<List<List<List<double>>>>()!;
    //     }
    //
    //     if (Coordinates is null) return null;
    //     throw new InvalidOperationException("Coordinates doesn't represent a Polygon");
    // }
}