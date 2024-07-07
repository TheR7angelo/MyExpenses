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

        var properties = placeSig.GetType().GetProperties();

        // XNamespace ns = "http://www.opengis.net/kml/2.2";
        //
        // var kml = new XDocument(
        //     new XDeclaration("1.0", "UTF-8", string.Empty),
        //     new XElement(ns + "kml",
        //         new XElement(ns + "Document", new XAttribute("id", "root_doc"),
        //             new XElement(ns + "Schema", new XAttribute("name", "location_data"),
        //                 new XAttribute("id", "location_data"),
        //                 new XElement(ns + "SimpleField", new XAttribute("name", "attribute1"),
        //                     new XAttribute("type", "string")),
        //                 new XElement(ns + "SimpleField", new XAttribute("name", "attribute2"),
        //                     new XAttribute("type", "int")),
        //                 new XElement(ns + "SimpleField", new XAttribute("name", "attribute3"),
        //                     new XAttribute("type", "string"))
        //             ),
        //             new XElement(ns + "Placemark",
        //                 new XElement(ns + "name", place.Name),
        //                 new XElement(ns + "ExtendedData",
        //                     new XElement(ns + "SchemaData", new XAttribute("schemaUrl", "#location_data"),
        //                         new XElement(ns + "SimpleData", new XAttribute("name", "attribute1"), "value1"),
        //                         new XElement(ns + "SimpleData", new XAttribute("name", "attribute2"), "123"),
        //                         new XElement(ns + "SimpleData", new XAttribute("name", "attribute3"), "value3")
        //                     )
        //                 ),
        //                 new XElement(ns + "Point",
        //                     new XElement(ns + "coordinates",
        //                         $"{place.Longitude!.Value.ToString(CultureInfo.InvariantCulture)},{place.Latitude!.Value.ToString(CultureInfo.InvariantCulture)},0")
        //                 )
        //             )
        //         )
        //     )
        // );
        //
        // kml.Save("location.kml");
    }
}