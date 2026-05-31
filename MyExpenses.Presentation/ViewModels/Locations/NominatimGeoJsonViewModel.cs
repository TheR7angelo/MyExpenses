using NetTopologySuite.Geometries;

namespace MyExpenses.Presentation.ViewModels.Locations;

public abstract class NominatimGeoJsonViewModel
{
    public string? Type { get; set; }

    protected abstract Geometry ToGeometry();

    public Point? CenterPoint => ToGeometry().Centroid;
}

public class NominatimPointViewModel : NominatimGeoJsonViewModel
{
    public List<double>? Coordinates { get; set; }

    protected override Geometry ToGeometry()
    {
        if (Coordinates is null || Coordinates.Count < 2) return Point.Empty;
        return new Point(Coordinates[0], Coordinates[1]);
    }
}

public class NominatimLineStringViewModel : NominatimGeoJsonViewModel
{
    public List<List<double>>? Coordinates { get; set; }

    protected override Geometry ToGeometry()
    {
        if (Coordinates == null || Coordinates.Count == 0)
            return LineString.Empty;

        var coords = Coordinates
            .Where(c => c.Count >= 2)
            .Select(c => new Coordinate(c[0], c[1]))
            .ToArray();

        return coords.Length is 0 ?
            LineString.Empty
            : new LineString(coords);
    }
}

public class NominatimPolygonViewModel : NominatimGeoJsonViewModel
{
    public List<List<List<double>>>? Coordinates { get; set; }

    protected override Geometry ToGeometry()
    {
        if (Coordinates == null || Coordinates.Count == 0 || Coordinates[0].Count == 0)
            return Polygon.Empty;

        var exteriorCoords = Coordinates[0]
            .Where(c => c.Count >= 2)
            .Select(c => new Coordinate(c[0], c[1]))
            .ToArray();

        if (exteriorCoords.Length is 0)
            return Polygon.Empty;

        var linearRing = new LinearRing(exteriorCoords);
        return new Polygon(linearRing);
    }
}