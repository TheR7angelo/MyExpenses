using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.Maps;

public class StreetViewTest
{
    private Point Point { get; } = new(-0.5754690669950785, 44.837624634089);

    [Fact]
    private void GoToGoogleEarthWeb()
    {
        Point.GoToGoogleEarthWeb();
    }

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
            .Select(s => s.Geometry).ToList();

        XNamespace ns = "http://www.opengis.net/kml/2.2";

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(ns + "kml",
                new XElement(ns + "Document")
            )
        );

        foreach (var point in points)
        {
            kml.Root!.Element(ns+"Document")!.Add(
                new XElement(ns + "Placemark",
                    new XElement(ns + "name", "Location"),
                    new XElement(ns + "Point",
                        new XElement(ns + "coordinates", $"{point.Y.ToString(CultureInfo.InvariantCulture)},{point.X.ToString(CultureInfo.InvariantCulture)},0")
                    )
                )
            );
        }

        kml.Save("location.kml");
    }

    [Fact]
    private void GoToKmlMultiPlace()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0).ToList();

        XNamespace ns = "https://www.opengis.net/kml/2.2";

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(ns + "kml",
                new XElement(ns + "Document")
            )
        );

        foreach (var place in places)
        {
            kml.Root!.Element(ns+"Document")!.Add(
                new XElement(ns + "Placemark",
                    new XElement(ns + "name", place.Name),
                    new XElement(ns + "Point",
                        new XElement(ns + "coordinates", $"{place.Longitude!.Value.ToString(CultureInfo.InvariantCulture)},{place.Latitude!.Value.ToString(CultureInfo.InvariantCulture)},0")
                    )
                )
            );
        }

        kml.Save("location.kml");
    }

    [Fact]
    private void GoToMaps()
    {
        var url =
            $"https://maps.google.com/maps?q={Point.Y.ToString(CultureInfo.InvariantCulture)},{Point.X.ToString(CultureInfo.InvariantCulture)}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    [Fact]
    private void GoToStreetView()
    {
        const int zoomLevel = 0;
        var url =
            $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={Point.Y.ToString(CultureInfo.InvariantCulture)}, {Point.X.ToString(CultureInfo.InvariantCulture)}&zoom={zoomLevel}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}