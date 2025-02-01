using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using AutoMapper;
using Mapsui.Layers;
using Mapsui.Projections;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Properties;

namespace MyExpenses.Models.AutoMapper.Profiles;

public class PointFeatureToTPlaceProfile : Profile
{
    public PointFeatureToTPlaceProfile()
    {
        CreateMap<TPlace, PointFeature>().ConvertUsing(PlaceToPointFeature);
        CreateMap<PointFeature, TPlace>().ConvertUsing(PointFeatureToTPlace);
    }

    private static TPlace PointFeatureToTPlace(PointFeature feature, TPlace tPlace)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var place = new TPlace();
        var properties = typeof(TPlace).GetProperties();
        foreach (var entry in feature.Fields)
        {
            var property = entry.GetPropertiesInfoByName<ColumnAttribute>(properties);
            if (property is null) continue;
            var value = feature[entry];
            if ((value is string str && !string.IsNullOrEmpty(str)) || value is not null)
            {
                try
                {
                    value = Convert.ChangeType(value,
                        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            property.SetValue(place, value);
        }

        return place;
    }

    private static PointFeature PlaceToPointFeature(TPlace place, PointFeature pointFeature)
    {
        var point = SphericalMercator.FromLonLat(place.Longitude ?? 0, place.Latitude ?? 0);
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var feature = new PointFeature(point.x, point.y);

        var properties = typeof(TPlace).GetProperties();
        foreach (var property in properties)
        {
            var columnName = property.GetCustomAttribute<ColumnAttribute>()?.Name;
            if (string.IsNullOrEmpty(columnName)) continue;

            feature[columnName] = property.GetValue(place);
        }

        return feature;
    }
}