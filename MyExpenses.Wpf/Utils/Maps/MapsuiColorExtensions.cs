using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace MyExpenses.Wpf.Utils.Maps;

public static class MapsuiColorExtensions
{
    public static global::Mapsui.Styles.Color ToColor(this SolidColorBrush solidBrush)
    {
        var baseColor = solidBrush.Color;
        return baseColor.ToColor();
    }

    public static global::Mapsui.Styles.Color ToColor(this Color baseColor)
    {
        var color = new global::Mapsui.Styles.Color(baseColor.R, baseColor.G, baseColor.B, baseColor.A);
        return color;
    }
}