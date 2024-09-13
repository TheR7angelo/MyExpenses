using System.Diagnostics;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleEarth
{
    public static Uri ToGoogleEarthWeb(this TPlace place, int altitudeLevel = 200)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleEarthWeb(altitudeLevel)!;
    }

    public static Uri ToGoogleEarthWeb(this Point point, int altitudeLevel = 200)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var googleEarthUrl = $"https://earth.google.com/web/@{yInvariant},{xInvariant},{altitudeLevel}a,0d,30y,0h,0t,0r";
        var uri = new Uri(googleEarthUrl);

        Process.Start(new ProcessStartInfo
        {
            FileName = googleEarthUrl,
            UseShellExecute = true
        });

        return uri;
    }
}