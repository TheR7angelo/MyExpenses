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

    public static string GetMaterialDesignBodyColorHexadecimal()
        => "MaterialDesignBody".GetHexadecimalCode();

    public static string GetMaterialDesignPaperColorHexadecimal()
        => "MaterialDesignPaper".GetHexadecimalCode();

    private static string GetHexadecimalCode(this string resourceKey)
    {
        var brush = (SolidColorBrush)resourceKey.GetFindResource();
        var wpfColor = brush.Color;
        var code = wpfColor.ToHexadecimal();

        return code;
    }
}