using System.Diagnostics;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleMaps
{
    public static Uri ToGoogleStreetView(this TPlace place, int zoomLevel = 0)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleStreetView(zoomLevel)!;
    }

    public static Uri ToGoogleStreetView(this Point point, int zoomLevel = 0)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={xInvariant}, {yInvariant}&zoom={zoomLevel}";
        var uri = new Uri(url);

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });

        return uri;
    }

    public static Uri ToGoogleMaps(this TPlace place)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleMaps()!;
    }

    public static Uri ToGoogleMaps(this Point point)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://maps.google.com/maps?q={xInvariant}, {yInvariant}";
        var uri = new Uri(url);

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });

        return uri;
    }
}