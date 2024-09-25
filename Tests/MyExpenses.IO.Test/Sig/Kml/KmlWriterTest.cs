using System.Reflection;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using NetTopologySuite.Geometries;

namespace MyExpenses.IO.Test.Sig.Kml;

public class KmlWriterTest
{
    private Point Point { get; } = new(-0.5754690669950785, 44.837624634089);

    [Fact]
    private void GoToKml()
    {
        const string fileSavePath = "location.kml";
        Point.ToKmlFile(fileSavePath);

        Assert.True(File.Exists(fileSavePath));
    }

    [Fact]
    private void GoToKmlMultiPoint()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        var points = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .Select(s => (Point)s.Geometry!).ToList();

        const string fileSavePath = "locations.kml";
        points.ToKmlFile(fileSavePath);

        Assert.True(File.Exists(fileSavePath));
    }

    [Fact]
    private void GoToKmlPlace()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        var place = context.TPlaces.First(s =>
            s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0);

        const string filename = "location place.kmz";

        place.ToKmlFile(filename);

        Assert.True(File.Exists(filename));
    }

    [Fact]
    private void GoToKmlPlaces()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        var places = context.TPlaces.Where(s =>
            s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0).ToList();

        const string filename = "location places.kml";

        places.ToKmlFile(filename);

        Assert.True(File.Exists(filename));
    }
}