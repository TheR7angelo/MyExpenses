using Mapsui;
using Mapsui.Animations;
using Mapsui.Layers;

namespace MyExpenses.Utils.Maps;

public static class ZoomUtils
{
    /// <summary>
    /// Adjusts the zoom level and center of a navigator based on the features within a writable layer.
    /// </summary>
    /// <param name="navigator">The navigator to apply the zoom and center adjustments.</param>
    /// <param name="writableLayer">The writable layer containing features to calculate the zoom area.</param>
    public static void SetZoom(this Navigator navigator, WritableLayer writableLayer)
    {
        var points = writableLayer.GetFeatures().Select(s => ((PointFeature)s).Point).ToArray();
        navigator.SetZoom(points);
    }

    /// <summary>
    /// Adjusts the zoom level of the navigator based on the provided points.
    /// If one point is provided, the navigator centers and zooms to that point.
    /// If multiple points are provided, the navigator zooms to fit all points within a bounding rectangle.
    /// </summary>
    /// <param name="navigator">The navigator instance to be adjusted.</param>
    /// <param name="points">The array of points to determine the zoom behavior. An empty array will result in no action.</param>
    public static void SetZoom(this Navigator navigator, MPoint[] points)
    {
        switch (points.Length)
        {
            case 0:
                break;
            case 1:
                navigator.CenterOnAndZoomTo(points[0]);
                break;
            case > 1:
                var mRect = points.ToMRect();
                navigator.ZoomToBox(mRect);
                break;
        }
    }

    /// <summary>
    /// Centers the navigator on a specific point and adjusts the zoom level to the specified resolution within an optional duration.
    /// </summary>
    /// <param name="navigator">The navigator instance to be adjusted.</param>
    /// <param name="center">The point to center the navigator on.</param>
    /// <param name="resolution">The zoom level resolution to apply. Default is 0.01.</param>
    /// <param name="duration">The duration of the animation in milliseconds. Default is -1, meaning no animation.</param>
    /// <param name="easing">The easing function to use for the animation. Default is null.</param>
    public static void CenterOnAndZoomTo(this Navigator navigator, MPoint center, double resolution = 0.01,
        long duration = -1,
        Easing? easing = null)
    {
        navigator.CenterOn(center, duration, easing);
        navigator.ZoomTo(resolution, duration, easing);
    }
}