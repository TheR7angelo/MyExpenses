namespace Domain.Models.Nominatium;

public class NominatimGeoJsonDomain
{
    public string? Type { get; set; }

    public IEnumerable<float>? Coordinates { get; set; }
}