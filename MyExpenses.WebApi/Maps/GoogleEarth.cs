using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using MyExpenses.Models.Sql.Tables;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleEarth
{
    public static string ToKmlFile(this Point point, string fileSavePath, string name = "Point")
    {
        fileSavePath = Path.ChangeExtension(fileSavePath, ".kml");

        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(ns + "kml",
                new XElement(ns + "Placemark",
                    new XElement(ns + "name", name),
                    new XElement(ns + "Point",
                        new XElement(ns + "coordinates",
                            $"{xInvariant}, {yInvariant},0")))));

        kml.Save(fileSavePath);

        return fileSavePath;
    }

    public static void GoToGoogleEarthWeb(this TPlace place, int altitudeLevel = 200)
        => place.Geometry.GoToGoogleEarthWeb(altitudeLevel);

    public static void GoToGoogleEarthWeb(this Point point, int altitudeLevel = 200)
    {
        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var googleEarthUrl = $"https://earth.google.com/web/@{yInvariant},{xInvariant},{altitudeLevel}a,0d,30y,0h,0t,0r";

        Process.Start(new ProcessStartInfo
        {
            FileName = googleEarthUrl,
            UseShellExecute = true
        });
    }

    private static (string YInvariant, string XInvariant) ToInvariantCoordinate(this Point point)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        return (yInvariant, xInvariant);
    }
}