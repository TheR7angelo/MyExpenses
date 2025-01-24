using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.SharedUtils;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Geometries;
using Serilog;

namespace MyExpenses.IO.Sig.Kml;

public static class KmlWriter
{
    /// <summary>
    /// Exports a collection of <see cref="ISig"/> geometries to a KML or KMZ file and saves it to the specified path.
    /// </summary>
    /// <param name="sigs">An enumerable collection of <see cref="ISig"/> instances containing the geometry data to be exported.</param>
    /// <param name="fileSavePath">The file path where the KML or KMZ file will be saved. The file extension must be ".kml" or ".kmz".</param>
    /// <param name="geomType">Optional. Specifies the type of geometry being exported, such as "Point". Defaults to "Point".</param>
    /// <returns>Returns true if the operation is successful; otherwise, returns false.</returns>
    public static bool ToKmlFile(this IEnumerable<ISig> sigs, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileSavePath);

        var enumerable = sigs as ISig[] ?? sigs.ToArray();
        enumerable = enumerable.Where(s => s.Geometry is not null).ToArray();
        var typeSig = sigs.GetType().GetGenericArguments()[0];
        var fields = Utils.GetFields(typeSig);

        var schemaElement = fields.CreateKmlSchema(filenameWithoutExtension);

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // Suppression applied because creating instances of XDocument and XElement involves
        // unavoidable memory allocations. Despite ReSharper's warning regarding performance,
        // these allocations are fundamental for building the KML document structure.
        // Optimizing this further is not practical without compromising functionality.
        // ReSharper warnings are temporarily disabled to prevent unnecessary distraction.
        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",

                new XElement(KmlUtils.KmlNamespace + "Document",
                    new XAttribute("id", "root_doc"),
                    schemaElement)));
        // ReSharper restore HeapView.ObjectAllocation.Evident

        var displayNameProperty = GetDisplayNameProperty(typeSig);

        foreach (var obj in enumerable.WithIndex())
        {
            var displayName = displayNameProperty is null
                ? $"{geomType} {obj.Index + 1}"
                : displayNameProperty.GetValue(obj.Element);

            var (xInvariant, yInvariant) = ((Point)obj.Element.Geometry!).ToInvariantCoordinate();

            var kmlAttribute = obj.Element.CreateKmlAttribute(filenameWithoutExtension);

            // ReSharper disable HeapView.ObjectAllocation.Evident
            // This suppression is applied because creating instances of XElement for the "Placemark" structure
            // unavoidably requires memory allocations. ReSharper warns about possible performance issues due to
            // these allocations, but they are essential for constructing the KML elements with the necessary attributes.
            // Disabling the warning here avoids unnecessary interruptions as these allocations are intentional.
            kml.Root!.Element(KmlUtils.KmlNamespace +"Document")!.Add(
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    kmlAttribute,
                    new XElement(KmlUtils.KmlNamespace + "name", displayName),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}")
                    )
                )
                // ReSharper restore HeapView.ObjectAllocation.Evident
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

    /// <summary>
    /// Exports an individual <see cref="ISig"/> geometry to a KML or KMZ file and saves it to the specified path.
    /// </summary>
    /// <param name="sig">The <see cref="ISig"/> instance containing the geometry data to be exported.</param>
    /// <param name="fileSavePath">The file path where the KML or KMZ file will be saved. The file extension must be ".kml" or ".kmz".</param>
    /// <param name="geomType">Optional. Specifies the type of geometry being exported, such as "Point". Defaults to "Point".</param>
    public static void ToKmlFile(this ISig sig, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(fileSavePath);
        var kmlAttribute = sig.CreateKmlAttribute(filenameWithoutExtension);

        var fields = Utils.GetFields(sig.GetType());
        var schemaElement = fields.CreateKmlSchema(filenameWithoutExtension);

        var (xInvariant, yInvariant) = ((Point)sig.Geometry!).ToInvariantCoordinate();

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // This suppression is applied because creating the XDocument and XElement objects for the KML structure
        // involves unavoidable memory allocations. ReSharper raises a warning about potential performance concerns,
        // but these allocations are necessary to build a valid KML file with the required elements such as Placemark,
        // Point, and their associated attributes. Optimizing these allocations is not feasible without compromising functionality.
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
        // ReSharper restore HeapView.ObjectAllocation.Evident

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    /// <summary>
    /// Exports a collection of <see cref="Point"/> geometries to a KML or KMZ file and saves it to the specified path.
    /// </summary>
    /// <param name="points">The collection of <see cref="Point"/> geometries to be exported.</param>
    /// <param name="fileSavePath">The file path where the KML or KMZ file will be saved. The file extension must be ".kml" or ".kmz".</param>
    /// <param name="geomType">Optional. Defines the type of geometry being exported. Defaults to "Point".</param>
    public static void ToKmlFile(this IEnumerable<Point> points, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // This suppression is applied because constructing the XDocument and XElement objects
        // inherently requires memory allocations. ReSharper warns about potential performance overhead,
        // but these allocations are essential for initializing the KML file structure.
        // This operation is necessary and cannot be optimized further.
        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Document")
            )
        );
        // ReSharper restore HeapView.ObjectAllocation.Evident

        foreach (var obj in points.WithIndex())
        {
            var indexedName = $"{geomType} {obj.Index + 1}";
            var (xInvariant, yInvariant) = obj.Element.ToInvariantCoordinate();

            // ReSharper disable HeapView.ObjectAllocation.Evident
            // Suppression applied because constructing XElement objects for the "Placemark" structure
            // and its child elements (e.g., name, coordinates) requires memory allocations. ReSharper raises warnings
            // about performance concerns, but these allocations are necessary for creating a valid KML structure.
            // This is intentional and integral to the functionality.
            kml.Root!.Element(KmlUtils.KmlNamespace +"Document")!.Add(
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    new XElement(KmlUtils.KmlNamespace + "name", indexedName),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}")
                    )
                )
            );
            // ReSharper restore HeapView.ObjectAllocation.Evident
        }

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    /// <summary>
    /// Exports a given <see cref="Point"/> geometry to a KML or KMZ file and saves it to the specified path.
    /// </summary>
    /// <param name="point">The <see cref="Point"/> geometry to be exported.</param>
    /// <param name="fileSavePath">The file path where the KML or KMZ file will be saved. The file extension must be ".kml" or ".kmz".</param>
    /// <param name="geomType">Optional. Defines the type of geometry being exported. Defaults to "Point".</param>
    public static void ToKmlFile(this Point point, string fileSavePath, string geomType = "Point")
    {
        var extension = Path.GetExtension(fileSavePath);
        extension.TestExtensionError();

        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // This suppression is applied because creating XDocument and XElement objects requires memory allocations.
        // These allocations are flagged by ReSharper as potential performance issues, but they are necessary parts
        // of constructing the KML file structure, including elements such as Placemark, Point, and coordinates.
        // These operations are fundamental and cannot be further optimized without compromising functionality.
        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    new XElement(KmlUtils.KmlNamespace + "name", geomType),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{xInvariant}, {yInvariant}")))));
        // ReSharper restore HeapView.ObjectAllocation.Evident

        SaveToKmlKmzFile(fileSavePath, kml, extension);
    }

    private static void TestExtensionError(this string extension)
    {
        extension = extension.ToLower();

        ReadOnlySpan<string> extensions = [".kml", ".kmz"];
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