using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.SharedUtils.Properties;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;

namespace MyExpenses.IO.Sig.Shp;

public static class ShapeWriter
{
    private const long MaxFileSize = 1_800_000_000; // Size Limit 1.8 GB
    private static readonly List<string> Extensions = ["shp", "shx", "dbf", "prj", "cpg"];

    /// <summary>
    /// Writes a collection of geometrical features to a Shapefile.
    /// </summary>
    /// <param name="features">A collection of ISig geometrical features to be written to the Shapefile.</param>
    /// <param name="savePath">The file path where the Shapefile will be saved.</param>
    /// <param name="projection">Optional. The projection data in WKT format to be included in the .prj file of the Shapefile.</param>
    /// <param name="encoding">Optional. The character encoding to be used for attribute data in the Shapefile.</param>
    /// <param name="shapeType">Optional. The specific ShapeType to use for writing geometries. If not specified, it defaults to the ShapeType of the first geometry.</param>
    /// <returns>Returns true if the Shapefile was successfully created and written, otherwise false.</returns>
    public static bool ToShapeFile(this IEnumerable<ISig> features, string savePath, string? projection = null,
        Encoding? encoding = null, ShapeType? shapeType = null)
    {
        try
        {
            var collection = features as ISig[] ?? features.ToArray();
            collection = collection.Where(s => s.Geometry is not null).ToArray();
            var typeSig = features.GetType().GetGenericArguments()[0];
            var fieldsDictionary = Utils.GetFields(typeSig);
            var fieldsArray = fieldsDictionary.Values.ToArray();

            var geomType = shapeType ?? collection.First().Geometry!.GetShapeType();

            // The creation of ShapefileWriterOptions is essential to configure shapefile writing.
            // It defines the ShapeType (geometry type) and the fields structure required for attributes.
            // This allocation is unavoidable and necessary to ensure accurate file writing with the
            // correct configuration for geometries and attributes fields.
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var options = new ShapefileWriterOptions((ShapeType)geomType!, fieldsArray)
            {
                Encoding = encoding,
                Projection = projection
            };

            var currentPartNumber = 1;
            var currentBasePath = AddPartSuffix(savePath, currentPartNumber);
            var shpWriter = Shapefile.OpenWrite(currentBasePath, options);
            foreach (var feature in collection)
            {
                var totalFileSize = GetCurrentTotalFileSize(currentBasePath);
                if (totalFileSize >= MaxFileSize)
                {
                    shpWriter?.Dispose();

                    currentPartNumber++;
                    currentBasePath = AddPartSuffix(savePath, currentPartNumber);
                    shpWriter = Shapefile.OpenWrite(currentBasePath, options);
                }

                shpWriter.Geometry = feature.Geometry;
                WriteFields(feature, fieldsDictionary);
                shpWriter.Write();
            }

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
        long finalSize = 0;
        foreach (var extension in Extensions)
        {
            finalSize += GetFileSize(basePath, extension);
        }
        return finalSize;
    }

    private static long GetFileSize(string basePath, string extension)
    {
        var filePath = Path.ChangeExtension(basePath, extension);

        var fileSize = File.Exists(filePath) ?
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The allocation of a FileInfo object is necessary here to retrieve the file's length.
            // While ReSharper warns about a potentially unnecessary allocation, this is the most
            // straightforward and platform-agnostic way to access the file size. FileInfo provides
            // a clean abstraction over platform-specific file system APIs, making this approach reliable
            // and maintainable. The code intentionally avoids premature optimizations or more complex
            // methods (e.g., P/Invoke or unsaf
            new FileInfo(filePath).Length
            : 0;
        return fileSize;
    }

    private static void WriteFields(ISig feature, Dictionary<string, DbfField> fieldsDictionary)
    {
        foreach (var (key, field) in fieldsDictionary)
        {
            field.Value = key.GetPropertiesInfoByName<ColumnAttribute>(feature);
        }
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