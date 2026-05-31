namespace MyExpenses.Application.Dtos.Nominatium;

public class NominatimGeoJsonDto
{
    public string? Type { get; set; }
}

public class NominatimPointDto : NominatimGeoJsonDto
{
    public List<double>? Coordinates { get; set; }
}

public class NominatimLineStringDto : NominatimGeoJsonDto
{
    public List<List<double>>? Coordinates { get; set; }
}

public class NominatimPolygonDto : NominatimGeoJsonDto
{
    public List<List<List<double>>>? Coordinates { get; set; }
}