using System.Windows.Media;
using MyExpenses.Wpf.Utils.Maps;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.Utils;

public static class Resources
{
    private static object GetFindResource(this string resourceKey)
    {
        var frameworkElement = new System.Windows.FrameworkElement();
        return frameworkElement.FindResource(resourceKey);
    }

    public static Mapsui.Styles.Color GetMaterialDesignPaperMapsUiStylesColor()
    {
        var brush = (SolidColorBrush)"MaterialDesignPaper".GetFindResource();
        var backColor = brush.ToMapsuiColor();

        return backColor;
    }

    public static SKColor GetMaterialDesignBodySkColor()
    {
        var brush = (SolidColorBrush)"MaterialDesignBody".GetFindResource();
        var wpfColor = brush.Color;
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }
}