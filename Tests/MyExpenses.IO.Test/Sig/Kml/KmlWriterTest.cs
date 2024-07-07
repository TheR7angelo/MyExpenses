using System.Globalization;
using System.Xml.Linq;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.Models.IO.Sig.Keyhole_Markup_Language;
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
        const string temp = @"C:\Users\Rapha\Documents\Programmation\MyExpenses\MyExpenses.Wpf\bin\Debug\net8.0-windows\Databases\Model - Using.sqlite";
        using var context = new DataBaseContext(temp);
        var points = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .Select(s => s.Geometry).ToList();

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
        var placeSig = mapping.Map<PlaceSig>(place);

        const string filename = "location place.kmz";

        placeSig.ToKmlFile(filename);

        Assert.True(File.Exists(filename));
    }

    private (string YInvariant, string XInvariant) ToInvariantCoordinate(Point point)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        return (yInvariant, xInvariant);
    }
}