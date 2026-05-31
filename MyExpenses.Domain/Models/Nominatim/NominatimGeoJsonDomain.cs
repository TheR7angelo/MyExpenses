namespace Domain.Models.Nominatim;

public class NominatimGeoJsonDomain
{
    public string? Type { get; set; }
}

public class NominatimPointDomain : NominatimGeoJsonDomain
{
    public List<double>? Coordinates { get; set; }
}

public class NominatimLineStringDomain : NominatimGeoJsonDomain
{
    public List<List<double>>? Coordinates { get; set; }
}

public class NominatimPolygonDomain : NominatimGeoJsonDomain
{
    public List<List<List<double>>>? Coordinates { get; set; }
}