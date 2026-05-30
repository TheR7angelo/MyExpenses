using System.Globalization;
using Mapsui;

namespace MyExpenses.Presentation.Converters;

/// <summary>
/// Provides methods to convert coordinates to a format that is invariant to regional settings.
/// This ensures consistent string representations of geographical points across different cultures.
/// </summary>
public static class InvariantCoordinateConverter
{
    /// <summary>
    /// Converts a MPoint to its invariant string representation, ensuring consistent formatting across different cultures.
    /// </summary>
    /// <param name="point">The MPoint to convert.</param>
    /// <returns>A tuple containing the X and Y coordinates as strings in an invariant format.</returns>
    public static (string XInvariant, string YInvariant) ToInvariantCoordinate(this MPoint point)
    {
        var xInvariant = point.X.ToString(CultureInfo.InvariantCulture);
        var yInvariant = point.Y.ToString(CultureInfo.InvariantCulture);

        return (xInvariant, yInvariant);
    }
}