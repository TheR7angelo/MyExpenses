using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
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
    private static readonly List<string> Extensions = ["shp", "shx", "dbf", "prj", "cpg"];

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
                WriteFields(feature, fieldsDictionary);
                shpWriter.Write();
            }

            WriteProjectionFile(currentBasePath, projection, encoding);
            shpWriter.Dispose();

            if (currentPartNumber.Equals(1)) CleanNames(currentBasePath);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static void CleanNames(string currentBasePath)
    {
        var baseDirectory = Path.GetDirectoryName(currentBasePath)!;
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(currentBasePath);

        foreach (var extension in Extensions)
        {
            var file = Path.Join(baseDirectory, $"{filenameWithoutExtension}.{extension}");
            if (!File.Exists(file)) continue;

            var newFileName = filenameWithoutExtension.Replace("_part1", string.Empty);
            var newFilePath = Path.Join(baseDirectory, $"{newFileName}.{extension}");
            File.Move(file, newFilePath, true);
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
        var fileSize = File.Exists(filePath) ? new FileInfo(filePath).Length : 0;
        return fileSize;
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

    private static void WriteFields(ISig feature, Dictionary<string, DbfField> fieldsDictionary)
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

            _ => throw new ArgumentOutOfRangeException(nameof(geometry), @"Unsupported geometry type")
        };
    }
}