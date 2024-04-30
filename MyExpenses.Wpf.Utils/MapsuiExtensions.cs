using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace MyExpenses.Wpf.Utils;

public static class MapsuiExtensions
{
    public static Mapsui.Styles.Color ToColor(this SolidColorBrush solidBrush)
    {
        var baseColor = solidBrush.Color;
        return baseColor.ToColor();
    }

    public static Mapsui.Styles.Color ToColor(this Color baseColor)
    {
        var color = new Mapsui.Styles.Color(baseColor.R, baseColor.G, baseColor.B, baseColor.A);
        return color;
    }
}