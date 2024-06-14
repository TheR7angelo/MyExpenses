using Mapsui;
using Mapsui.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Utils.Maps;

public static class Converter
{
    /// <summary>
    /// Converts a TPlace object to an MPoint object.
    /// </summary>
    /// <param name="place">The TPlace object to convert.</param>
    /// <returns>The converted MPoint object.</returns>
    public static MPoint ToMPoint(this TPlace place)
    {
        var mapper = Mapping.Mapper;
        var pointFeature = mapper.Map<PointFeature>(place);
        return pointFeature.Point;
    }

    /// <summary>
    /// Converts an IFeature object to a TPlace object.
    /// </summary>
    /// <param name="feature">The IFeature object to convert.</param>
    /// <returns>The converted TPlace object.</returns>
    public static TPlace ToTPlace(this IFeature feature)
    {
        var mapper = Mapping.Mapper;
        var place = mapper.Map<TPlace>(feature);
        return place;
    }
}