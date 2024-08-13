using System.Windows.Media;
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

    public static SKColor GetMaterialDesignBodySkColor()
    {
        var brush = (SolidColorBrush)"MaterialDesignBody".GetFindResource();
        var wpfColor = brush.Color;
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }
}