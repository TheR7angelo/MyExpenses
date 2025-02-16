using Mapsui.Layers;
using Mapsui.UI.Maui;
using MyExpenses.Utils.Maps;

namespace MyExpenses.Maui.Utils.Maps;

public static class ZoomUtils
{
    public static void SetZoom(this MapControl mapControl, WritableLayer writableLayer)
    {
        var points = writableLayer.GetFeatures().Select(s => ((PointFeature)s).Point).ToArray();

        switch (points.Length)
        {
            case 0:
                break;
            case 1:
                mapControl.Map.Navigator.CenterOnAndZoomTo(points[0], 1);
                break;
            case > 1:
                var mRect = points.ToMRect();
                mapControl.Map.Navigator.ZoomToBox(mRect);
                break;
        }
    }
}