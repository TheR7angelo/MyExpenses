namespace Domain.Models.Nominatim;

public class NominatimGeoJsonDomain
{
    public string? Type { get; set; }

    public IEnumerable<float>? Coordinates { get; set; }
}