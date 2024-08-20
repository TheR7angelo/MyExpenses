using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            var collection = features.ToList();

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
                        var multiLineString = new MultiLineString(new[] { lineString });
                        shpWriter.Geometry = multiLineString;
                        break;
                    }
                    case Polygon polygon:
                    {
                        var multiPolygon = new MultiPolygon(new[] { polygon });
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
        var fieldCreators = new Dictionary<Type, Func<string, int?, int?, DbfField>>
        {
            [typeof(int)] = (name, maxLength, _)
                => maxLength == null ? new DbfNumericInt32Field(name) : new DbfNumericInt32Field(name, maxLength.Value),
            [typeof(long)] = (name, maxLength, _)
                => maxLength == null ? new DbfNumericInt64Field(name) : new DbfNumericInt64Field(name, maxLength.Value),
            [typeof(double)] = (name, maxLength, precision)
                =>
            {
                if (maxLength is null && precision is null)
                {
                    return new DbfFloatField(name);
                }

                if (maxLength is null && precision is not null)
                {
                    return new DbfFloatField(name, 19, (int)precision);
                }

                if (maxLength is not null && precision is null)
                {
                    return new DbfFloatField(name, (int)maxLength, 0);
                }

                return new DbfFloatField(name, (int)maxLength!, (int)precision!);
            },
            [typeof(string)] = (name, maxLength, _)
                => maxLength == null ? new DbfCharacterField(name) : new DbfCharacterField(name, maxLength.Value),
            [typeof(DateTime)] = (name, _, _)
                => new DbfDateField(name),
            [typeof(bool)] = (name, _, _)
                => new DbfLogicalField(name)
        };

        var properties = feature.GetType().GetProperties();
        var fields = new Dictionary<string, DbfField>();

        foreach (var property in properties)
        {
            if (property.GetValueByProperty<ColumnAttribute>() is not string name) continue;

            if (name.Length > 10) name = name[..10];

            var maxLength = property.GetValueByProperty<MaxLengthAttribute>() as int?;
            var precision = property.GetValueByProperty<PrecisionAttribute>() as int?;
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (!fieldCreators.TryGetValue(type, out var fieldCreator))
            {
                //TODO work
                throw new ArgumentOutOfRangeException(nameof(property), "Pas de field de prévu");
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

            _ => throw new ArgumentOutOfRangeException(nameof(geometry), "Unsupported geometry type.")
        };
    }
}