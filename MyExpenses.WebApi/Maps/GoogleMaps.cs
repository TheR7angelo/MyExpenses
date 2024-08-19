using System.Diagnostics;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleMaps
{
    public static void ToGoogleStreetView(this TPlace place, int zoomLevel = 0)
    {
        var point = place.Geometry as Point;
        point?.ToGoogleStreetView(zoomLevel);
    }

    public static void ToGoogleStreetView(this Point point, int zoomLevel = 0)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={xInvariant}, {yInvariant}&zoom={zoomLevel}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    public static void ToGoogleMaps(this TPlace place)
    {
        var point = place.Geometry as Point;
        point?.ToGoogleMaps();
    }

    public static void ToGoogleMaps(this Point point)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://maps.google.com/maps?q={xInvariant}, {yInvariant}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}