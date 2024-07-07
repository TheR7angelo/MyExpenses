using System.Globalization;
using System.IO.Compression;
using System.Xml.Linq;
using NetTopologySuite.Geometries;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlWriter
{
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
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Document")
            )
        );

        foreach (var obj in points.Select((point, i) => new { i, point }))
        {
            var indexedName = $"{name} {obj.i + 1}";
            var (yInvariant, xInvariant) = obj.point.ToInvariantCoordinate();

            kml.Root!.Element(KmlUtils.KmlNamespace +"Document")!.Add(
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    new XElement(KmlUtils.KmlNamespace + "name", indexedName),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}")
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
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    new XElement(KmlUtils.KmlNamespace + "name", name),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{xInvariant}, {yInvariant}")))));

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