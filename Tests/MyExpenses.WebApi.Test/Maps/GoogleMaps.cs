using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Test.Maps;

public class StreetViewTest
{
    private Point Point { get; } = new(-0.5754690669950785, 44.837624634089);

    [Fact]
    private void GoToGoogleEarthWeb()
    {
        Point.GoToGoogleEarthWeb();
    }

    [Fact]
    private void GoToMaps()
    {
        var url =
            $"https://maps.google.com/maps?q={Point.Y.ToString(CultureInfo.InvariantCulture)},{Point.X.ToString(CultureInfo.InvariantCulture)}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    [Fact]
    private void GoToStreetView()
    {
        const int zoomLevel = 0;
        var url =
            $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={Point.Y.ToString(CultureInfo.InvariantCulture)}, {Point.X.ToString(CultureInfo.InvariantCulture)}&zoom={zoomLevel}";

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}