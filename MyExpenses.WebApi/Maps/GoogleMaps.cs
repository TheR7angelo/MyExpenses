using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleMaps
{
    public static string ToGoogleStreetView(this TPlace place, int zoomLevel = 0)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleStreetView(zoomLevel)!;
    }

    public static string ToGoogleStreetView(this Point point, int zoomLevel = 0)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={xInvariant}, {yInvariant}&zoom={zoomLevel}";

        url.StartProcess();

        return url;
    }

    public static string ToGoogleMaps(this TPlace place)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleMaps()!;
    }

    public static string ToGoogleMaps(this Point point)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://maps.google.com/maps?q={xInvariant}, {yInvariant}";

        url.StartProcess();

        return url;
    }
}