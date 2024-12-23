using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;

namespace MyExpenses.IO.Sig.Shp;

public static class ShapeWriter
{
    public static bool ToShapeFile(this IEnumerable<ISig> features, string savePath, string? projection = null,
        Encoding? encoding = null, ShapeType? shapeType = null)
    {
        try
        {
            var collection = features.Where(s => s.Geometry is not null).ToList();

            var geomType = shapeType ?? collection.First(s => s.Geometry is not null).Geometry!.GetShapeType();

            var fieldsDictionary = collection.First().GetFields();
            var fieldsArray = fieldsDictionary.Select(s => s.Value).ToArray();

            var options = new ShapefileWriterOptions((ShapeType)geomType!, fieldsArray);

            using var shpWriter = Shapefile.OpenWrite(savePath, options);

            foreach (var feature in collection)
            {
                switch (feature.Geometry)
                {
                    case LineString lineString:
                    {
                        var multiLineString = new MultiLineString([lineString]);
                        shpWriter.Geometry = multiLineString;
                        break;
                    }
                    case Polygon polygon:
                    {
                        var multiPolygon = new MultiPolygon([polygon]);
                        shpWriter.Geometry = multiPolygon;
                        break;
                    }
                    default:
                        shpWriter.Geometry = feature.Geometry;
                        break;
                }

                foreach (var key in fieldsDictionary.Keys)
                {
                    var value = key.GetPropertiesInfoByName<ColumnAttribute>(feature);

                    var field = fieldsDictionary[key];
                    field.Value = value;
                }

                shpWriter.Write();
            }

            if (string.IsNullOrWhiteSpace(projection)) return true;

            var prjFilePath = Path.ChangeExtension(savePath, "prj");
            File.WriteAllText(prjFilePath, projection, encoding ?? Encoding.UTF8);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return false;
        }
    }

    private static Dictionary<string, DbfField> GetFields(this ISig feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature), "Feature cannot be null");

        var fieldCreators = new Dictionary<Type, Func<string, int?, int?, DbfField>>
        {
            [typeof(int)] = (name, maxLength, _) =>
                maxLength == null ? new DbfNumericInt32Field(name) : new DbfNumericInt32Field(name, maxLength.Value),
            [typeof(long)] = (name, maxLength, _) =>
                maxLength == null ? new DbfNumericInt64Field(name) : new DbfNumericInt64Field(name, maxLength.Value),
            [typeof(double)] = (name, maxLength, precision) =>
                new DbfFloatField(name, maxLength ?? 19, precision ?? 0),
            [typeof(string)] = (name, maxLength, _) =>
                new DbfCharacterField(name, maxLength ?? 255), // Par défaut 255 caractères max.
            [typeof(DateTime)] = (name, _, _) => new DbfDateField(name),
            [typeof(bool)] = (name, _, _) => new DbfLogicalField(name)
        };

        var fields = new Dictionary<string, DbfField>();

        foreach (var property in feature.GetType().GetProperties())
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null || string.IsNullOrWhiteSpace(columnAttribute.Name)) continue;

            var name = columnAttribute.Name;
            if (name.Length > 10) name = name[..10];

            var maxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            var precision = property.GetCustomAttribute<PrecisionAttribute>()?.Precision;

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (!fieldCreators.TryGetValue(propertyType, out var fieldCreator))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(propertyType),
                    $"Type not supported for the property '{property.Name}' : {propertyType.Name}"
                );
            }

            fields[name] = fieldCreator(name, maxLength, precision);
        }

        return fields;
    }

    private static ShapeType? GetShapeType(this Geometry geometry)
    {
        return geometry switch
        {
            Point => ShapeType.Point,
            MultiPoint => ShapeType.MultiPoint,
            LineString => ShapeType.PolyLine,
            MultiLineString => ShapeType.PolyLine,
            Polygon => ShapeType.Polygon,
            MultiPolygon => ShapeType.Polygon,

            _ => throw new ArgumentOutOfRangeException(nameof(geometry), "Unsupported geometry type")
        };
    }
}