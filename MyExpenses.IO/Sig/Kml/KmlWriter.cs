using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Geometries;
using Serilog;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlWriter
{
    public static bool ToKmlFile(this IEnumerable<ISig> sigs, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileSavePath);

        var enumerable = sigs as ISig[] ?? sigs.ToArray();
        enumerable = enumerable.Where(s => s.Geometry is not null).ToArray();
        var fields = enumerable.First().GetFields();

        var schemaElement = fields.CreateKmlSchema(filenameWithoutExtension);

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",

                new XElement(KmlUtils.KmlNamespace + "Document",
                    new XAttribute("id", "root_doc"),
                    schemaElement)));

        var typeSig = sigs.GetType().GetGenericArguments()[0];
        var displayNameProperty = GetDisplayNameProperty(typeSig);

        foreach (var obj in enumerable.Select((point, i) => new { i, point }))
        {
            var displayName = displayNameProperty is null
                ? $"{geomType} {obj.i + 1}"
                : displayNameProperty.GetValue(obj.point);

            var (xInvariant, yInvariant) = ((Point)obj.point.Geometry!).ToInvariantCoordinate();

            var kmlAttribute = obj.point.CreateKmlAttribute(filenameWithoutExtension);

            kml.Root!.Element(KmlUtils.KmlNamespace +"Document")!.Add(
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    kmlAttribute,
                    new XElement(KmlUtils.KmlNamespace + "name", displayName),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}")
                    )
                )
            );
        }

        return SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    private static PropertyInfo? GetDisplayNameProperty(Type typeSig)
    {
        const string displayName = "name";
        var displayNameProperty = displayName.GetPropertiesInfoByName<ColumnAttribute>(typeSig);
        return displayNameProperty;
    }

    public static void ToKmlFile(this ISig sig, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileSavePath);
        var kmlAttribute = sig.CreateKmlAttribute(filenameWithoutExtension);

        var fields = sig.GetFields();
        var schemaElement = fields.CreateKmlSchema(filenameWithoutExtension);

        var (xInvariant, yInvariant) = ((Point)sig.Geometry!).ToInvariantCoordinate();

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",

                new XElement(KmlUtils.KmlNamespace + "Document",
                    new XAttribute("id", "root_doc"),
                    schemaElement,
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    kmlAttribute,
                    new XElement(KmlUtils.KmlNamespace + "name", geomType),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}"))))));

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    public static void ToKmlFile(this IEnumerable<Point> points, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Document")
            )
        );

        foreach (var obj in points.Select((point, i) => new { i, point }))
        {
            var indexedName = $"{geomType} {obj.i + 1}";
            var (xInvariant, yInvariant) = obj.point.ToInvariantCoordinate();

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

    public static void ToKmlFile(this Point point, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    new XElement(KmlUtils.KmlNamespace + "name", geomType),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{xInvariant}, {yInvariant}")))));

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    private static void TestExtensionError(this string extension)
    {
        extension = extension.ToLower();

        var extensions = new List<string> { ".kml", ".kmz" };
        if (!extensions.Contains(extension))
        {
            throw new ArgumentException($"The file extension must be .kml or .kmz. The provided extension was {extension}.");
        }
    }

    private static bool SaveToKmlKmzFile(string fileSavePath, XDocument kml, string extension)
    {
        try
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            tempFilePath = Path.ChangeExtension(tempFilePath, ".kml");
            kml.Save(tempFilePath);

            if (extension is ".kmz")
            {
                if (File.Exists(fileSavePath)) File.Delete(fileSavePath);

                using var zip = ZipFile.Open(fileSavePath, ZipArchiveMode.Create);
                zip.CreateEntryFromFile(tempFilePath, Path.GetFileName(tempFilePath));

                File.Delete(tempFilePath);
            }
            else
            {
                File.Move(tempFilePath, fileSavePath, true);
            }

            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while saving KML file");
            return false;
        }
    }
}