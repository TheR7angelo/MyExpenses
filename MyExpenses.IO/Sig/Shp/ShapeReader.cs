using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using GeoReso.Models.IO.Shape.Converters;
using MyExpenses.IO.Csv;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Models.IO.Sig.Shp;
using MyExpenses.Models.IO.Sig.Shp.Converters;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;
using Serilog;

namespace MyExpenses.IO.Sig.Shp;

public static class ShapeReader
{
    private const string GeometryColumn = "Geometry";
    private static readonly DateTimeConverter DateTimeConverterObject = new();

    public static List<GeographicCoordinateSystem> GeographicCoordinateSystems { get; }

    static ShapeReader()
    {
        var csvFile = Path.GetFullPath("Resources");
        csvFile = Path.Combine(csvFile, "Projections", "ProjectionSystem.csv");
        GeographicCoordinateSystems = csvFile.ReadCsv<GeographicCoordinateSystem>().ToList();
    }

    public static (List<T> Features, string? Projection) ReadShapeFile<T>(this string filePath) where T : class
    {
        //TODO work
        Log.Information("Lecture du shape \"{FilePath}\"", filePath);

        string? projection = null;
        GeographicCoordinateSystem? geographicCoordinateSystem = null;
        var prjFilePath = Path.ChangeExtension(filePath, "prj");
        if (File.Exists(prjFilePath))
        {
            //TODO work
            Log.Information("Tentative de lecture du fichier .prj");

            projection = File.ReadAllText(prjFilePath);
            geographicCoordinateSystem = GetGeographicCoordinateSystemFromPrj(projection);

            //TODO work
            Log.Information("Système trouvé => {Name}", geographicCoordinateSystem?.Name);
        }
        else
        {
            //TODO work
            Log.Warning("Fichier .prj introuvable");
        }

        var features = filePath.ReadFeatures(geographicCoordinateSystem);

        //TODO work
        Log.Information("Shape \"{FilePath}\" lu, Nombre entité: {NbCount}", filePath, features.Count);

        return (features.ToList<T>(), projection);
    }

    public static (List<Feature> Features, string? Projection) ReadShapeFile(this string filePath)
    {
        //TODO work
        Log.Information("Lecture du shape \"{FilePath}\"", filePath);

        string? projection = null;
        GeographicCoordinateSystem? geographicCoordinateSystem = null;
        var prjFilePath = Path.ChangeExtension(filePath, "prj");
        if (File.Exists(prjFilePath))
        {
            //TODO work
            Log.Information("Tentative de lecture du fichier .prj");

            projection = File.ReadAllText(prjFilePath);
            geographicCoordinateSystem = GetGeographicCoordinateSystemFromPrj(projection);

            //TODO work
            Log.Information("Système trouvé => {Name}", geographicCoordinateSystem?.Name);
        }
        else
        {
            //TODO work
            Log.Warning("Fichier .prj introuvable");
        }

        var features = filePath.ReadFeatures(geographicCoordinateSystem);

        //TODO work
        Log.Information("Shape \"{FilePath}\" lu, Nombre entité: {NbCount}", filePath, features.Count);

        return (features, projection);
    }

    private static GeographicCoordinateSystem? GetGeographicCoordinateSystemFromPrj(this string projectionString)
        => GeographicCoordinateSystems.FirstOrDefault(s => s.Prj.Equals(projectionString));


    private static List<T> ToList<T>(this IEnumerable<Feature> features) where T : class
    {
        var results = new List<T>();

        foreach (var feature in features)
        {
            var tmp = (ISig)Activator.CreateInstance<T>();
            tmp.Geometry = feature.Geometry;

            foreach (var name in feature.Attributes.GetNames())
            {
                var tmpName = name;
                if (tmpName.Length > 10) tmpName = tmpName[..10];

                var property = tmpName.GetPropertiesInfoByName<T, ColumnAttribute>();
                if (property is null) continue;
                if (!property.CanWrite) continue;

                var value = feature.Attributes[tmpName];

                try
                {
                    property.SetValue(tmp, value);
                }
                catch (Exception)
                {
                    var targetType = property.PropertyType;
                    try
                    {
                        value = value.ConvertTo(targetType);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{name}");
                        Console.WriteLine(e);
                        throw;
                    }

                    try
                    {
                        property.SetValue(tmp, value);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"{name}");
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }

            results.Add((tmp as T)!);
        }

        return results;
    }

    private static List<Feature> ReadFeatures(this string filePath,
        GeographicCoordinateSystem? geographicCoordinateSystem = null)
    {
        var geometryFactory = new GeometryFactory();
        if (geographicCoordinateSystem is not null)
        {
            geometryFactory = new GeometryFactory(new PrecisionModel(), geographicCoordinateSystem.WellKnownId);
        }

        var features = new List<Feature>();

        using var reader = new ShapefileDataReader(filePath, geometryFactory);
        while (reader.Read())
        {
            try
            {
                var geom = (Geometry)reader[GeometryColumn];
                if (!geom.IsValid)
                {
                    geom = GeometryFixer.Fix(geom);
                }

                var feature = new Feature
                {
                    Geometry = geom,
                    Attributes = new AttributesTable()
                };

                AddAttributes(reader, feature);
                features.Add(feature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return features;
    }

    private static void AddAttributes(IDataRecord reader, IFeature feature)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            if (name == GeometryColumn) continue;

            var value = reader[name];
            if (value is string str)
            {
                str = str.Trim('*', ' ');
                value = string.IsNullOrEmpty(str) ? null : str;
                // value = value is not null ? DateTimeConverterObject.ConvertFromString((string?)value) : value;
                var dateConvert = value is not null ? DateTimeConverterObject.ConvertFromString((string?)value) : value;

                value = dateConvert ?? value;
            }

            feature.Attributes.Add(name, value);
        }
    }
    // private static ShapefileReaderOptions GetShapeReaderOption()
    // {
    //     var opts = new ShapefileReaderOptions
    //     {
    //         GeometryBuilderMode = GeometryBuilderMode.FixInvalidShapes
    //     };
    //
    //     return opts;
    // }
}