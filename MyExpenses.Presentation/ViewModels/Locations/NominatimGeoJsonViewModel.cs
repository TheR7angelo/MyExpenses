namespace MyExpenses.Presentation.ViewModels.Locations;

public class NominatimGeoJsonViewModel
{
    public string? Type { get; set; }
}

public class NominatimPointViewModel : NominatimGeoJsonViewModel
{
    public List<double>? Coordinates { get; set; }
}

public class NominatimLineStringViewModel : NominatimGeoJsonViewModel
{
    public List<List<double>>? Coordinates { get; set; }
}

public class NominatimPolygonViewModel : NominatimGeoJsonViewModel
{
    public List<List<List<double>>>? Coordinates { get; set; }
}