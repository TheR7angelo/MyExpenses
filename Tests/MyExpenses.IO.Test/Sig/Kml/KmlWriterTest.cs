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
        using var context = new DataBaseContext();
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

        const string filename = "location.kml";
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
        var fields = placeSig.GetFields();
        var schemaElement = fields.CreateKmlSchema(filenameWithoutExtension);
        var kmlAttribute = placeSig.CreateKmlAttribute(filenameWithoutExtension);

        var (yInvariant, xInvariant) = ToInvariantCoordinate(place.Geometry);

        var kml = new XDocument(
            new XDeclaration("1.0", "UTF-8", string.Empty),
            new XElement(KmlUtils.KmlNamespace + "kml",

                new XElement(KmlUtils.KmlNamespace + "Document",
                    new XAttribute("id", "root_doc"),
                    schemaElement,
                new XElement(KmlUtils.KmlNamespace + "Placemark",
                    kmlAttribute,
                    new XElement(KmlUtils.KmlNamespace + "name", place.Name),
                    new XElement(KmlUtils.KmlNamespace + "Point",
                        new XElement(KmlUtils.KmlNamespace + "coordinates",
                            $"{yInvariant}, {xInvariant}"))))));

        kml.Save(filename);
    }

    private (string YInvariant, string XInvariant) ToInvariantCoordinate(Point point)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        return (yInvariant, xInvariant);
    }
}