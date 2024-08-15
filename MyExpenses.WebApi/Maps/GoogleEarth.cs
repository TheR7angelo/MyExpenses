using System.Diagnostics;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleEarth
{
    public static void ToGoogleEarthWeb(this TPlace place, int altitudeLevel = 200)
        => place.Geometry?.ToGoogleEarthWeb(altitudeLevel);

    public static void ToGoogleEarthWeb(this Point point, int altitudeLevel = 200)
    {
        var (yInvariant, xInvariant) = point.ToInvariantCoordinate();

        var googleEarthUrl = $"https://earth.google.com/web/@{xInvariant},{yInvariant},{altitudeLevel}a,0d,30y,0h,0t,0r";
        Console.WriteLine(googleEarthUrl);

        Process.Start(new ProcessStartInfo
        {
            FileName = googleEarthUrl,
            UseShellExecute = true
        });
    }
}