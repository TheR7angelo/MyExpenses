using System.Globalization;
using NetTopologySuite.Geometries;

namespace MyExpenses.Utils.Maps;

public static class InvariantCoordinate
{
    public static (string YInvariant, string XInvariant) ToInvariantCoordinate(this Point point)
    {
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);

        return (yInvariant, xInvariant);
    }
}