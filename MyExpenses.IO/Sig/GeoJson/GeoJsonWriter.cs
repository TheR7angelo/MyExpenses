using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MyExpenses.Models.IO.Sig.Interfaces;
using NetTopologySuite.Features;
using Newtonsoft.Json;

namespace MyExpenses.IO.Sig.GeoJson;

public static class GeoJsonWriter
{
    public static void ToGeoJson(this IEnumerable<ISig> features, string savePath)
    {
        savePath = Path.ChangeExtension(savePath, ".geojson");

        var enumerable = features as ISig[] ?? features.ToArray();
        var typeSig = features.GetType().GetGenericArguments()[0];
        var properties = typeSig.GetProperties();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The FeatureCollection is instantiated only once in this context,
        // and its allocation is both necessary and expected as part of the GeoJSON generation process.
        // The memory allocation here is proportional to the collection being processed and does not
        // lead to unnecessary overhead.
        var featureCollection = new FeatureCollection();
        foreach (var feature in enumerable)
        {
            if (feature.Geometry is null) continue;

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // A new instance of AttributesTable is required for each Feature
            // because each geographic entity (Feature) has its own unique set of attributes.
            // Reusing the same table would cause conflicts and lead to inconsistent data between Features.
            // This allocation is essential to ensure data integrity and adhere to the expected
            // separation of data between entities.
            var attributeTable = new AttributesTable();
            foreach (var propertyInfo in properties)
            {
                var columnName = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name;
                if (string.IsNullOrEmpty(columnName)) continue;

                attributeTable.Add(columnName, propertyInfo.GetValue(feature, null));
            }

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // A new instance of Feature is required for each entity, as each Feature
            // is a unique combination of a Geometry and its associated AttributesTable. Reusing
            // the same Feature instance across entities would lead to data conflicts and
            // inconsistencies. Creating a new Feature instance for each entity ensures data integrity
            // and proper separation of each feature's data.
            var temp = new Feature(feature.Geometry, attributeTable);
            featureCollection.Add(temp);
        }

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // A new instance of StringWriter and JsonTextWriter is created for each serialization
        // operation because these objects are short-lived and designed for temporary use.
        // The `using` statement ensures proper disposal of resources like buffers or streams.
        // Similarly, a new instance of GeoJsonWriter is initialized as it is used exclusively for a single operation in this context.
        // This guarantees correct isolation between serialization operations and simplifies the code.
        // Reusing these instances would add unnecessary complexity without a significant benefit.
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        jsonWriter.Formatting = Formatting.Indented;

        var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
        // ReSharper restore HeapView.ObjectAllocation.Evident
        geoJsonWriter.Write(featureCollection, jsonWriter);

        File.WriteAllText(savePath, stringWriter.ToString());
    }
}