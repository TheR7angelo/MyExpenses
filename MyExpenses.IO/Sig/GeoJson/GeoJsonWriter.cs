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

        var featureCollection = new FeatureCollection();
        foreach (var feature in enumerable)
        {
            if (feature.Geometry is null) continue;

            var attributeTable = new AttributesTable();
            foreach (var propertyInfo in properties)
            {
                var columnName = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name;
                if (string.IsNullOrEmpty(columnName)) continue;

                attributeTable.Add(columnName, propertyInfo.GetValue(feature, null));
            }

            featureCollection.Add(new Feature(feature.Geometry, attributeTable));
        }

        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        jsonWriter.Formatting = Formatting.Indented;

        var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
        geoJsonWriter.Write(featureCollection, jsonWriter);

        File.WriteAllText(savePath, stringWriter.ToString());
    }
}