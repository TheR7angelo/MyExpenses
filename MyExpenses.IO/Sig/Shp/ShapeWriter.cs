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
    private const long MaxFileSize = 1_800_000_000; // Size Limit 1.8 GB

    public static bool ToShapeFile(this IEnumerable<ISig> features, string savePath, string? projection = null,
        Encoding? encoding = null, ShapeType? shapeType = null)
    {
        try
        {
            var enumerable = features.ToList();

            var options = GetShapefileWriterOptions(enumerable, shapeType);

            var collection = enumerable.ToList();
            var fieldsDictionary = collection.First().GetFields();

            var part = 1;
            var partSavePath = GetPartSavePath(savePath, part);
            ShapefileWriter? shpWriter = null;

            foreach (var feature in collection)
            {
                shpWriter ??= Shapefile.OpenWrite(partSavePath, options);

                WriteGeometry(shpWriter, feature);
                WriteFields(shpWriter, feature, fieldsDictionary);

                shpWriter = CheckFileSizeAndSplit(shpWriter, ref partSavePath, ref part, savePath);
            }

            shpWriter?.Dispose();

            WriteProjectionFile(savePath, projection, encoding);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static void WriteProjectionFile(string savePath, string? projection, Encoding? encoding)
    {
        if (string.IsNullOrWhiteSpace(projection)) return;

        var prjFilePath = Path.ChangeExtension(savePath, "prj");
        File.WriteAllText(prjFilePath, projection, encoding ?? Encoding.UTF8);
    }

    private static ShapefileWriter? CheckFileSizeAndSplit(ShapefileWriter? shpWriter, ref string partSavePath, ref int part, string savePath)
    {
        var totalSize = GetCurrentTotalFileSize(partSavePath);
        if (totalSize <= MaxFileSize) return shpWriter;

        shpWriter?.Dispose();
        shpWriter = null;
        part++;
        partSavePath = GetPartSavePath(savePath, part);
        return shpWriter;
    }

    private static void WriteFields(ShapefileWriter shpWriter, ISig feature, Dictionary<string, DbfField> fieldsDictionary)
    {
        foreach (var key in fieldsDictionary.Keys)
        {
            var value = key.GetPropertiesInfoByName<ColumnAttribute>(feature);
            var field = fieldsDictionary[key];
            field.Value = value;
        }
        shpWriter.Write();
    }

    private static void WriteGeometry(ShapefileWriter shpWriter, ISig feature)
    {
        switch (feature.Geometry)
        {
            case LineString lineString:
                shpWriter.Geometry = new MultiLineString([lineString]);
                break;
            case Polygon polygon:
                shpWriter.Geometry = new MultiPolygon([polygon]);
                break;
            default:
                shpWriter.Geometry = feature.Geometry;
                break;
        }
    }

    private static ShapefileWriterOptions GetShapefileWriterOptions(IEnumerable<ISig> features, ShapeType? shapeType)
    {
        var collection = features.ToList();
        var geomType = shapeType ?? collection.First(s => s.Geometry is not null).Geometry!.GetShapeType();
        var fieldsArray = collection.First().GetFields().Select(s => s.Value).ToArray();
        return new ShapefileWriterOptions((ShapeType)geomType, fieldsArray);
    }

    private static string GetPartSavePath(string originalPath, int partNumber)
    {
        return Path.ChangeExtension(originalPath, null) + $"_part{partNumber}.shp";
    }

    /// <summary>
    /// Calculates the current total file size for a shapefile dataset by summing up
    /// the sizes of the associated .shp, .dbf, and .shx files.
    /// </summary>
    /// <param name="basePath">The base file path of the shapefile, without file extension.</param>
    /// <returns>The total file size in bytes as a long.</returns>
    private static long GetCurrentTotalFileSize(string basePath)
    {
        return GetFileSize(basePath, "shp") + GetFileSize(basePath, "dbf") + GetFileSize(basePath, "shx");
    }

    private static long GetFileSize(string basePath, string extension)
    {
        var filePath = Path.ChangeExtension(basePath, extension);
        return File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
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
            var name = property.GetCustomAttribute<ColumnAttribute>()?.Name;
            if (string.IsNullOrEmpty(name)) continue;

            if (name.Length > 10) name = name[..10];

            var maxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            var precision = property.GetCustomAttribute<PrecisionAttribute>()?.Precision;
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