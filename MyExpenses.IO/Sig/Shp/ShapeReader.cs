using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Models.IO.Sig.Shp.Converters;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO.Esri;
using Serilog;

namespace MyExpenses.IO.Sig.Shp;

public static class ShapeReader
{
    private static readonly DateTimeConverter DateTimeConverterObject = new();

    public static (List<T> Features, string? Projection) ReadShapeFile<T>(this string filePath) where T : class, ISig
    {
        //TODO work
        Log.Information("Lecture du shape \"{FilePath}\"", filePath);

        string? projection = null;
        var prjFilePath = Path.ChangeExtension(filePath, "prj");
        if (File.Exists(prjFilePath))
        {
            //TODO work
            Log.Information("Tentative de lecture du fichier .prj");

            projection = File.ReadAllText(prjFilePath);
            var geographicCoordinateSystem = GetGeographicCoordinateSystemFromPrj(projection);

            //TODO work
            Log.Information("Système trouvé => {Name}", geographicCoordinateSystem?.AuthName);
        }
        else
        {
            //TODO work
            Log.Warning("Fichier .prj introuvable");
        }

        var features = filePath.ReadFeatures();

        //TODO work
        Log.Information("Shape \"{FilePath}\" lu, Nombre entité: {NbCount}", filePath, features.Count);

        return (features.ToList<T>(), projection);
    }

    private static TSpatialRefSy? GetGeographicCoordinateSystemFromPrj(this string projectionString)
    {
        using var context = new DataBaseContext(DbContextBackup.LocalFilePathDataBaseModel);
        return context.TSpatialRefSys.FirstOrDefault(s => s.Proj4text.Equals(projectionString));
    }

    private static List<T> ToList<T>(this IEnumerable<Feature> features) where T : class, ISig
    {
        var results = new List<T>();

        foreach (var feature in features)
        {
            var instance = Activator.CreateInstance<T>();
            instance.Geometry = feature.Geometry;

            ProcessAttributes(feature, instance);
            results.Add(instance);
        }

        return results;
    }

    private static void ProcessAttributes<T>(Feature feature, T instance) where T : class
    {
        foreach (var attributeName in feature.Attributes.GetNames())
        {
            var truncatedName = TruncateName(attributeName);
            var property = truncatedName.GetPropertiesInfoByName<T, ColumnAttribute>();

            if (property == null || !property.CanWrite) continue;

            var value = feature.Attributes[truncatedName];
            SetPropertyValue(property, instance, value);
        }
    }

    private static string TruncateName(string name)
    {
        return name.Length > 10 ? name[..10] : name;
    }

    private static void SetPropertyValue<T>(PropertyInfo property, T instance, object? value)
    {
        try
        {
            property.SetValue(instance, value);
        }
        catch (Exception)
        {
            try
            {
                var convertedValue = value?.ConvertTo(property.PropertyType);
                property.SetValue(instance, convertedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@$"Error assigning {property.Name} : {ex.Message}");
                throw;
            }
        }
    }

    private static List<Feature> ReadFeatures(this string filePath, TSpatialRefSy? spatialRef = null)
    {
        var features = new List<Feature>();

        foreach (var feature in Shapefile.ReadAllFeatures(filePath))
        {
            if (!feature.Geometry.IsValid)
            {
                feature.Geometry = GeometryFixer.Fix(feature.Geometry);
            }

            if (spatialRef is not null) feature.Geometry.SRID = spatialRef.Srid;
            var newFeature = feature.CleanFeature();
            features.Add(newFeature);
        }

        return features;
    }

    private static Feature CleanFeature(this Feature feature)
    {
        var newFeature = new Feature
        {
            Geometry = feature.Geometry,
            Attributes = new AttributesTable()
        };

        foreach (var name in feature.Attributes.GetNames())
        {
            var value = feature.Attributes[name];
            if (value is string str)
            {
                str = str.Trim('*', ' ');
                value = string.IsNullOrEmpty(str) ? null : str;
                // ReSharper disable once HeapView.BoxingAllocation
                var dateConvert = value is not null ? DateTimeConverterObject.ConvertFromString((string?)value) : value;

                value = dateConvert ?? value;
            }

            newFeature.Attributes.Add(name, value);
        }

        return newFeature;
    }
}