using Mapsui;
using Mapsui.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Utils;

public static class Converter
{
    public static TPlace ToTPlace(this IFeature feature)
    {
        var mapper = Mapping.Mapper;
        var place = mapper.Map<TPlace>(feature);
        return place;
    }

    public static PointFeature ToPointFeature(this TPlace place)
    {
        var mapper = Mapping.Mapper;
        var pointFeature = mapper.Map<PointFeature>(place);
        return pointFeature;
    }
}