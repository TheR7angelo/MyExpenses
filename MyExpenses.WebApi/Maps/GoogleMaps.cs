using System.Diagnostics;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleMaps
{
    public static void ToGoogleStreetView(this TPlace place, int zoomLevel = 0)
        => place.Geometry?.ToGoogleStreetView(zoomLevel);

    public static void ToGoogleStreetView(this Point point, int zoomLevel = 0)
    {
        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var url = $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={xInvariant}, {yInvariant}&zoom={zoomLevel}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    public static void ToGoogleMaps(this TPlace place)
        => place.Geometry?.ToGoogleMaps();

    public static void ToGoogleMaps(this Point point)
    {
        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var url = $"https://maps.google.com/maps?q={xInvariant}, {yInvariant}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}