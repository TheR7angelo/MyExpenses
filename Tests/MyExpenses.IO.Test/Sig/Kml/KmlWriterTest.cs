using MyExpenses.IO.Sig.Kml;
using MyExpenses.Sql.Context;
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
        using var context = new DataBaseContext();
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
        using var context = new DataBaseContext();
        var place = context.TPlaces.First(s =>
            s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0);

        var mapping = Models.AutoMapper.Mapping.Mapper;
        var placeSig = mapping.Map<ExportTPlace>(place);

        const string filename = "location place.kmz";

        placeSig.ToKmlFile(filename);

        Assert.True(File.Exists(filename));
    }

    [Fact]
    private void GoToKmlPlaces()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.Where(s =>
            s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0).ToList();

        var mapping = Models.AutoMapper.Mapping.Mapper;
        var placeSigs = places.Select(s => mapping.Map<ExportTPlace>(s));

        const string filename = "location places.kml";

        placeSigs.ToKmlFile(filename);

        Assert.True(File.Exists(filename));
    }
}