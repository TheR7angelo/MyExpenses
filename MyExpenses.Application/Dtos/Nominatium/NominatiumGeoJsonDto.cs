namespace MyExpenses.Application.Dtos.Nominatium;

public class NominatiumGeoJsonDto
{
    public string? Type { get; set; }

    public IEnumerable<float>? Coordinates { get; set; }
}