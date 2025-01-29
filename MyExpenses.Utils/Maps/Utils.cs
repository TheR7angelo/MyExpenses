using Mapsui;

namespace MyExpenses.Utils.Maps;

public static class Utils
{
    /// <summary>
    /// Converts an array of points into an MRect structure, representing a rectangle
    /// that encompasses all the points, optionally applying a margin.
    /// </summary>
    /// <param name="points">An array of points that define the boundaries of the rectangle.</param>
    /// <param name="margin">An optional margin percentage to expand the boundaries of the rectangle. Default is 10.</param>
    /// <returns>An MRect structure that encompasses all the points with the applied margin.</returns>
    public static MRect ToMRect(this MPoint[] points, double margin = 10)
    {
        double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
        double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

        var width = maxX - minX;
        var height = maxY - minY;

        var marginX = width * margin / 100;
        var marginY = height * margin / 100;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The mRect instance is used to store the coordinates of the rectangle to zoom to.
        return new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);
    }

    /// <summary>
    /// Converts an enumerable collection of points into an MRect structure, representing a rectangle
    /// that encompasses all the points, optionally applying a margin.
    /// </summary>
    /// <param name="points">An enumerable collection of points that define the boundaries of the rectangle.</param>
    /// <param name="margin">An optional margin percentage to expand the boundaries of the rectangle. Default is 10.</param>
    /// <returns>An MRect structure that encompasses all the points with the applied margin.</returns>
    public static MRect ToMRect(this IEnumerable<MPoint> points, double margin = 10)
        => ToMRect(points.ToArray(), margin);
}