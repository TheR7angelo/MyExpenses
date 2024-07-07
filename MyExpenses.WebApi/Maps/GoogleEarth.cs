using System.Diagnostics;
using System.Globalization;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleEarth
{
    public static void GoToGoogleEarthWeb(this Point point, int altitudeLevel = 200)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        var googleEarthUrl = $"https://earth.google.com/web/@{yInvariant},{xInvariant},{altitudeLevel}a,0d,30y,0h,0t,0r";

        Process.Start(new ProcessStartInfo
        {
            FileName = googleEarthUrl,
            UseShellExecute = true
        });
    }
}