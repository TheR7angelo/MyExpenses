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
        var brush = "MaterialDesignPaper".GetSolidColorBrush();
        var backColor = brush.ToMapsuiColor();

        return backColor;
    }

    public static SKColor GetMaterialDesignBodySkColor()
    {
        var wpfColor = "MaterialDesignBody".GetColor();
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }

    private static SolidColorBrush GetSolidColorBrush(this string resourceKey)
        => (SolidColorBrush)resourceKey.GetFindResource();

    private static Color GetColor(this string resourceKey)
        => ((SolidColorBrush)resourceKey.GetFindResource()).Color;

    public static string GetMaterialDesignBodyColorHexadecimal()
        => "MaterialDesignBody".GetHexadecimalCode();

    public static string GetMaterialDesignPaperColorHexadecimal()
        => "MaterialDesignPaper".GetHexadecimalCode();

    private static string GetHexadecimalCode(this string resourceKey)
    {
        var wpfColor = resourceKey.GetColor();
        var code = wpfColor.ToHexadecimal();

        return code;
    }
}