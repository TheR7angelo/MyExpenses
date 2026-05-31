using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyExpenses.WebApi.Entities;

[JsonConverter(typeof(NominatimGeoJsonConverter))]
public class NominatimGeoJson
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class NominatimPoint : NominatimGeoJson
{
    [JsonPropertyName("coordinates")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public List<double>? Coordinates { get; set; }
}

public class NominatimLineString : NominatimGeoJson
{
    [JsonPropertyName("coordinates")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public List<List<double>>? Coordinates { get; set; }
}

public class NominatimPolygon : NominatimGeoJson
{
    [JsonPropertyName("coordinates")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public List<List<List<double>>>? Coordinates { get; set; }
}

public class NominatimGeoJsonConverter : JsonConverter<NominatimGeoJson>
{
    private static readonly Type[] DerivedTypes =
    [
        typeof(NominatimPoint),
        typeof(NominatimLineString),
        typeof(NominatimPolygon)
    ];

    public override NominatimGeoJson? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        string? typeValue = null;
        if (root.TryGetProperty("type", out var typeProp) || root.TryGetProperty("Type", out typeProp))
        {
            typeValue = typeProp.GetString();
        }

        Type? targetType = null;
        if (typeValue != null)
        {
            foreach (var type in DerivedTypes)
            {
                if (!type.Name.EndsWith(typeValue, StringComparison.OrdinalIgnoreCase)) continue;
                targetType = type;
                break;
            }
        }

        var result = targetType != null
            ? root.Deserialize(targetType, options) as NominatimGeoJson
            : root.Deserialize<NominatimGeoJson>(options) ?? new NominatimGeoJson();

        return result;
    }

    public override void Write(Utf8JsonWriter writer, NominatimGeoJson value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize<object>(writer, value, options);
    }
}