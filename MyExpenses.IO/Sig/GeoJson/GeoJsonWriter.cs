using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.IO.Sig.Interfaces;
using MyExpenses.Utils.Properties;
using NetTopologySuite.Features;

namespace MyExpenses.IO.Sig.GeoJson;

public static class GeoJsonWriter
{
    public static void ToGeoJson(this IEnumerable<ISig> features, string savePath)
    {
        savePath = Path.ChangeExtension(savePath, ".geojson");
        var iEnumerable = features.ToList();

        var featureCollection = new FeatureCollection();
        var properties = iEnumerable.First().GetType().GetProperties();
        foreach (var feature in iEnumerable)
        {
            if (feature.Geometry is null) continue;

            var attributeTable = new AttributesTable();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetValueByProperty<ColumnAttribute>() is not string columnName) continue;

                attributeTable.Add(columnName, propertyInfo.GetValue(feature, null));
            }

            featureCollection.Add(new Feature(feature.Geometry, attributeTable));
        }

        var geoJsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
        var geoJson = geoJsonWriter.Write(featureCollection);

        File.WriteAllText(savePath, geoJson);
    }
}