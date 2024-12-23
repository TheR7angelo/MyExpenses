﻿using System.ComponentModel.DataAnnotations;
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
    private static readonly List<string> Extensions = ["shp", "shx", "dbf"];

    public static bool ToShapeFile(this IEnumerable<ISig> features, string savePath, string? projection = null,
        Encoding? encoding = null, ShapeType? shapeType = null)
    {
        try
        {
            var collection = features.Where(s => s.Geometry != null).ToList();

            var geomType = shapeType ?? collection.First().Geometry!.GetShapeType();
            var fieldsDictionary = collection.First().GetFields();
            var fieldsArray = fieldsDictionary.Values.ToArray();

            var options = new ShapefileWriterOptions((ShapeType)geomType!, fieldsArray);

            var currentPartNumber = 1;
            var currentBasePath = AddPartSuffix(savePath, currentPartNumber);
            var shpWriter = Shapefile.OpenWrite(currentBasePath, options);
            foreach (var feature in collection)
            {
                var totalFileSize = GetCurrentTotalFileSize(currentBasePath);
                if (totalFileSize >= MaxFileSize)
                {
                    shpWriter?.Dispose();
                    WriteProjectionFile(currentBasePath, projection, encoding);

                    currentPartNumber++;
                    currentBasePath = AddPartSuffix(savePath, currentPartNumber);
                    shpWriter = Shapefile.OpenWrite(currentBasePath, options);
                }

                WriteGeometry(shpWriter, feature);
                WriteFields(shpWriter, feature, fieldsDictionary);
                shpWriter.Write();
            }

            WriteProjectionFile(currentBasePath, projection, encoding);
            shpWriter.Dispose();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static string AddPartSuffix(string basePath, int currentPartNumber)
    {
        var directory = Path.GetDirectoryName(basePath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(basePath);
        var extension = Path.GetExtension(basePath);
        return Path.Combine(directory ?? string.Empty, $"{fileNameWithoutExtension}_part{currentPartNumber}{extension}");
    }

    private static long GetCurrentTotalFileSize(string basePath)
    {
        var finalSize = Extensions.Sum(extension => GetFileSize(basePath, extension));
        return finalSize;
    }

    private static long GetFileSize(string basePath, string extension)
    {
        var filePath = Path.ChangeExtension(basePath, extension);
        return File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
    }

    private static void WriteGeometry(ShapefileWriter shpWriter, ISig feature)
    {
        shpWriter.Geometry = feature.Geometry switch
        {
            LineString lineString => new MultiLineString([lineString]),
            Polygon polygon => new MultiPolygon([polygon]),
            _ => feature.Geometry
        };
    }

    private static void WriteFields(ShapefileWriter shpWriter, ISig feature,
        Dictionary<string, DbfField> fieldsDictionary)
    {
        foreach (var (key, field) in fieldsDictionary)
        {
            field.Value = key.GetPropertiesInfoByName<ColumnAttribute>(feature);
        }
    }

    private static void WriteProjectionFile(string savePath, string? projection, Encoding? encoding)
    {
        if (string.IsNullOrWhiteSpace(projection)) return;

        var prjFilePath = Path.ChangeExtension(savePath, "prj");
        File.WriteAllText(prjFilePath, projection, encoding ?? Encoding.UTF8);
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