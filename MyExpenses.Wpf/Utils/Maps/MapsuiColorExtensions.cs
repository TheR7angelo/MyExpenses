using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace MyExpenses.Wpf.Utils.Maps;

public static class MapsuiColorExtensions
{
    public static global::Mapsui.Styles.Color ToMapsuiColor(this SolidColorBrush solidBrush)
    {
        var baseColor = solidBrush.Color;
        return baseColor.ToMapsuiColor();
    }

    private static Mapsui.Styles.Color ToMapsuiColor(this Color baseColor)
    {
        var color = new Mapsui.Styles.Color(baseColor.R, baseColor.G, baseColor.B, baseColor.A);
        return color;
    }
}