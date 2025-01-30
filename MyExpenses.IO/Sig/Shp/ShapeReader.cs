using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Converters;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Objects;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO.Esri;
using Serilog;

namespace MyExpenses.IO.Sig.Shp;

public static class ShapeReader
{
    /// <summary>
    /// Reads a shapefile from the specified file path and extracts the features and projection information.
    /// </summary>
    /// <typeparam name="T">The type of the features to be read, which must implement the ISig interface.</typeparam>
    /// <param name="filePath">The file path of the shapefile to be read.</param>
    /// <returns>A tuple containing a list features of type T and an optional string representing the projection.</returns>
    public static (List<T> Features, string? Projection) ReadShapeFile<T>(this string filePath) where T : class, ISig
    {
        Log.Information("Reading file \"{FilePath}\"", filePath);

        string? projection = null;
        var prjFilePath = Path.ChangeExtension(filePath, "prj");
        if (File.Exists(prjFilePath))
        {
            Log.Information("Attempting to read the .prj file");

            projection = File.ReadAllText(prjFilePath);
            var geographicCoordinateSystem = GetGeographicCoordinateSystemFromPrj(projection);

            Log.Information("System found => {Name}", geographicCoordinateSystem?.AuthName);
        }
        else
        {
            Log.Warning(".prj file not found");
        }

        var features = filePath.ReadFeatures();
        var enumerable = features as Feature[] ?? features.ToArray();

        Log.Information("Shape \"{FilePath}\" read, Number of entities: {NbCount}", filePath, enumerable.Length.ToString());

        return (enumerable.ToList<T>().ToList(), projection);
    }

    private static TSpatialRefSy? GetGeographicCoordinateSystemFromPrj(this string projectionString)
    {
        // Creating a new DataBaseContext instance is required to access the database.
        // This usage is expected and unavoidable as each call represents a discrete transactional context.
        // The "using" statement ensures proper disposal of the context after use.
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext(DatabaseInfos.LocalFilePathDataBaseModel);
        return context.TSpatialRefSys.FirstOrDefault(s => s.Proj4text.Equals(projectionString));
    }

    private static IEnumerable<T> ToList<T>(this IEnumerable<Feature> features) where T : class, ISig
    {
        var enumerable = features as Feature[] ?? features.ToArray();

        foreach (var feature in enumerable)
        {
            var instance = Activator.CreateInstance<T>();
            instance.Geometry = feature.Geometry;

            ProcessAttributes(feature, instance);
            yield return instance;
        }
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
        var compiledSetter = PropertyAccessorCache<T>.CreateSetter(property);
        try
        {
            compiledSetter(instance, value);
        }
        catch (Exception)
        {
            try
            {
                var convertedValue = value?.ConvertTo(property.PropertyType);
                compiledSetter(instance, convertedValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@$"Error assigning {property.Name} : {ex.Message}");
                throw;
            }
        }
    }

    private static IEnumerable<Feature> ReadFeatures(this string filePath, TSpatialRefSy? spatialRef = null)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The "features" list is required to store processed and cleaned Feature objects.
        // This allocation is intentional and necessary to accumulate the results
        // before returning them to the caller. The scope is limited to this method,
        // ensuring no memory leaks.
        // var features = new List<Feature>();

        foreach (var feature in Shapefile.ReadAllFeatures(filePath))
        {
            if (!feature.Geometry.IsValid)
            {
                feature.Geometry = GeometryFixer.Fix(feature.Geometry);
            }

            if (spatialRef is not null) feature.Geometry.SRID = spatialRef.Srid;
            var newFeature = feature.CleanFeature();
            // features.Add(newFeature);
            yield return newFeature;
        }

        // return features;
    }

    private static Feature CleanFeature(this Feature feature)
    {
        // Creating a new "Feature" instance is necessary to encapsulate a cleaned geometry
        // and a new attributes table. This allocation ensures isolation from the original
        // object and avoids unintended side effects. The scope of this new object is
        // limited to the current processing logic.
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var newFeature = new Feature
        {
            Geometry = feature.Geometry,
            Attributes = new AttributesTable()
        };
        // ReSharper restore HeapView.ObjectAllocation.Evident

        foreach (var name in feature.Attributes.GetNames())
        {
            var value = feature.Attributes[name];
            if (value is string str)
            {
                str = str.Trim('*', ' ');
                value = string.IsNullOrEmpty(str) ? null : str;
                // ReSharper disable once HeapView.BoxingAllocation
                var dateConvert = value is not null ? ((string?)value).ConvertFromString() : value;

                value = dateConvert ?? value;
            }

            newFeature.Attributes.Add(name, value);
        }

        return newFeature;
    }
}