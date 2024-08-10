using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyExpenses.Models.WebApi.Nominatim;

public class NominatiumGeoJson
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("coordinates")]
    public object? Coordinates { get; set; }

    public List<List<double>> GetLineStringCoordinates()
    {
        if (Type?.ToLower() is "linestring" && Coordinates is JArray jArray)
        {
            return jArray.ToObject<List<List<double>>>()!;
        }
        throw new InvalidOperationException("Coordinates doesn't represent a LineString");
    }

    public List<List<List<double>>> GetPolygonCoordinates()
    {
        if (Type?.ToLower() is "polygon" && Coordinates is JArray jArray)
        {
            return jArray.ToObject<List<List<List<double>>>>()!;
        }
        throw new InvalidOperationException("Coordinates doesn't represent a Polygon");
    }
}