using Mapsui;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Utils.Maps;

public static class Converter
{
    public static TPlace ToTPlace(this IFeature feature)
    {
        var mapper = Mapping.Mapper;
        var place = mapper.Map<TPlace>(feature);
        return place;
    }
}