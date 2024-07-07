using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using NetTopologySuite.Geometries;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlWriter
{
    private static XNamespace KmlNamespace => XNamespace.Get("http://www.opengis.net/kml/2.2");

    public static void ToKmlFile(this IEnumerable<Point> points, string fileSavePath, string name = "Point")
    {
        var extension = Path.GetExtension(fileSavePath).ToLower();

        var extensions = new List<string> { ".kml", ".kmz" };
        if (!extensions.Contains(extension))
        {
            throw new ArgumentException($"The file extension must be .kml or .kmz. The provided extension was {extension}.");
        }

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlNamespace + "kml",
                new XElement(KmlNamespace + "Document")
            )
        );

        foreach (var obj in points.Select((point, i) => new { i, point }))
        {
            var indexedName = $"{name} {obj.i + 1}";
            var (yInvariant, xInvariant) = obj.point.ToInvariantCoordinate();

            kml.Root!.Element(KmlNamespace +"Document")!.Add(
                new XElement(KmlNamespace + "Placemark",
                    new XElement(KmlNamespace + "name", indexedName),
                    new XElement(KmlNamespace + "Point",
                        new XElement(KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}, 0")
                    )
                )
            );
        }

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    public static void ToKmlFile(this Point point, string fileSavePath, string name = "Point")
    {
        var extension = Path.GetExtension(fileSavePath).ToLower();

        var extensions = new List<string> { ".kml", ".kmz" };
        if (!extensions.Contains(extension))
        {
            throw new ArgumentException($"The file extension must be .kml or .kmz. The provided extension was {extension}.");
        }

        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlNamespace + "kml",
                new XElement(KmlNamespace + "Placemark",
                    new XElement(KmlNamespace + "name", name),
                    new XElement(KmlNamespace + "Point",
                        new XElement(KmlNamespace + "coordinates",
                            $"{xInvariant}, {yInvariant}, 0")))));

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    private static void SaveToKmlKmzFile(string fileSavePath, XDocument kml, string extension)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        tempFilePath = Path.ChangeExtension(tempFilePath, ".kml");
        kml.Save(tempFilePath);

        if (extension == ".kmz")
        {
            using var zip = ZipFile.Open(fileSavePath, ZipArchiveMode.Create);
            zip.CreateEntryFromFile(tempFilePath, Path.GetFileName(tempFilePath));

            File.Delete(tempFilePath);
        }
        else
        {
            File.Move(tempFilePath, fileSavePath, true);
        }
    }

    private static (string YInvariant, string XInvariant) ToInvariantCoordinate(this Point point)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        return (yInvariant, xInvariant);
    }
}