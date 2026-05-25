using System.Globalization;

namespace MyExpenses.Presentation.Converters;

public static class InvariantCoordinateConverter
{
    public static (string XInvariant, string YInvariant) ToInvariantCoordinate(this (double X, double Y) coordinate)
    {
        var xInvariant = coordinate.X.ToString(CultureInfo.InvariantCulture);
        var yInvariant = coordinate.Y.ToString(CultureInfo.InvariantCulture);

        return (xInvariant, yInvariant);
    }
}