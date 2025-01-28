using System.Windows.Media;
using MyExpenses.Wpf.Utils.Maps;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.Utils;

public static class Resources
{
    /// <summary>
    /// Retrieves the resource object associated with the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the resource objects to retrieve.</param>
    /// <returns>The resource object associated with the specified key.</returns>
    private static object GetFindResource(this string resourceKey)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var frameworkElement = new System.Windows.FrameworkElement();
        return frameworkElement.FindResource(resourceKey);
    }

    /// <summary>
    /// Retrieves the Material Design Paper color for Mapsui styles.
    /// </summary>
    /// <returns>The Material Design Paper color for Mapsui styles.</returns>
    public static Mapsui.Styles.Color GetMaterialDesignPaperMapsUiStylesColor()
    {
        var brush = "MaterialDesignPaper".GetSolidColorBrush();
        var backColor = brush.ToMapsuiColor();

        return backColor;
    }

    /// <summary>
    /// Retrieves the SkiaSharp color object associated with the MaterialDesignBody resource.
    /// </summary>
    /// <returns>The SkiaSharp color object associated with the MaterialDesignBody resource.</returns>
    public static SKColor GetMaterialDesignBodySkColor()
    {
        var wpfColor = "MaterialDesignBody".GetColor();
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }

    /// <summary>
    /// Retrieves the SolidColorBrush object associated with the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the resource objects to retrieve.</param>
    /// <returns>The SolidColorBrush object associated with the specified key.</returns>
    private static SolidColorBrush GetSolidColorBrush(this string resourceKey)
        => (SolidColorBrush)resourceKey.GetFindResource();

    /// <summary>
    /// Retrieves the color object associated with the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the color object to retrieve.</param>
    /// <returns>The color object associated with the specified key.</returns>
    private static Color GetColor(this string resourceKey)
        => ((SolidColorBrush)resourceKey.GetFindResource()).Color;

    /// <summary>
    /// Retrieves the hexadecimal code associated with the color object of the MaterialDesignBody resource.
    /// </summary>
    /// <returns>The hexadecimal code associated with the SkiaSharp color object of the MaterialDesignBody resource.</returns>
    public static string GetMaterialDesignBodyColorHexadecimal()
        => "MaterialDesignBody".GetHexadecimalCode();

    /// <summary>
    /// Retrieves the hexadecimal code associated with the color object of the MaterialDesignBody resource, excluding the alpha channel.
    /// </summary>
    /// <returns>The hexadecimal color code of the specified resource key, excluding the alpha channel, as a string.</returns>
    public static string GetMaterialDesignBodyColorHexadecimalWithoutAlpha()
        => "MaterialDesignBody".GetHexadecimalCodeWithoutAlpha();

    /// <summary>
    /// Retrieves the hexadecimal code associated with the color object of the MaterialDesignPaper resource.
    /// </summary>
    /// <returns>The hexadecimal code associated with the color object of the MaterialDesignPaper resource.</returns>
    public static string GetMaterialDesignPaperColorHexadecimal()
        => "MaterialDesignPaper".GetHexadecimalCode();

    /// <summary>
    /// Retrieves the hexadecimal code associated with the color object of the MaterialDesignPaper resource, excluding the alpha channel.
    /// </summary>
    /// <returns>The hexadecimal color code of the specified resource key, excluding the alpha channel, as a string.</returns>
    public static string GetMaterialDesignPaperColorHexadecimalWithoutAlpha()
        => "MaterialDesignPaper".GetHexadecimalCodeWithoutAlpha();

    /// <summary>
    /// Retrieves the hexadecimal code associated with the color object of the specified resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the resource object to retrieve the hexadecimal code for.</param>
    /// <returns>The hexadecimal code associated with the color object of the specified resource key.</returns>
    private static string GetHexadecimalCode(this string resourceKey)
    {
        var wpfColor = resourceKey.GetColor();
        var code = wpfColor.ToHexadecimal();

        return code;
    }

    /// <summary>
    /// Retrieves the hexadecimal color code of the specified resource key, excluding the alpha channel.
    /// </summary>
    /// <param name="resourceKey">The key of the resource object to retrieve the hexadecimal color code from.</param>
    /// <returns>The hexadecimal color code of the specified resource key, excluding the alpha channel, as a string.</returns>
    private static string GetHexadecimalCodeWithoutAlpha(this string resourceKey)
    {
        var wpfColor = resourceKey.GetColor();
        var code = wpfColor.ToHexadecimalWithoutAlpha();

        return code;
    }
}