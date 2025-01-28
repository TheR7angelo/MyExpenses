using System.Globalization;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MyExpenses.Utils;

public static class ColorExtensions
{
    /// <summary>
    /// Converts HSV components to RGB components.
    /// </summary>
    /// <param name="hue">The hue value (0-360).</param>
    /// <param name="saturation">The saturation value (0-1).</param>
    /// <param name="value">The brightness value (0-1).</param>
    /// <returns>A tuple representing the RGB components as doubles (range 0-1).</returns>
    public static (double r, double g, double b) ConvertHsvToRgb(double hue, double saturation, double value)
    {
        const double tolerance = 0.00001;

        if (Math.Abs(hue - 360.0) < tolerance) hue = 0;

        hue /= 60;

        var i = (int)Math.Floor(hue);
        var f = hue - i;

        var p = value * (1.0 - saturation);
        var q = value * (1.0 - saturation * f);
        var t = value * (1.0 - saturation * (1.0 - f));

        return i switch
        {
            0 => (value, t, p),
            1 => (q, value, p),
            2 => (p, value, t),
            3 => (p, q, value),
            4 => (t, p, value),
            _ => (value, p, q)
        };
    }

    /// <summary>
    /// Converts a hexadecimal color code to a SkiaSharp.SKColor object.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color code to convert.</param>
    /// <returns>The SkiaSharp.SKColor object representing the converted color.</returns>
    public static SKColor? ToSkColor(this string hexColor)
    {
        var success = SKColor.TryParse(hexColor, out var color);
        return success ? color : null;
    }

    /// <summary>
    /// Converts a hexadecimal color code to a SolidColorPaint object.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color code to convert.</param>
    /// <returns>A SolidColorPaint object representing the converted color, or null if the input is invalid.</returns>
    public static SolidColorPaint? ToSolidColorPaint(this string? hexColor)
    {
        var solidColorPaint = !string.IsNullOrEmpty(hexColor) &&
                              hexColor.ToSkColor() is { } skColor
            ? skColor.ToSolidColorPaint()
            : null;

        return solidColorPaint;
    }

    /// <summary>
    /// Converts an SKColor object to a SolidColorPaint object.
    /// </summary>
    /// <param name="skColor">The SKColor object to convert.</param>
    /// <returns>A SolidColorPaint object representing the specified SKColor.</returns>
    public static SolidColorPaint ToSolidColorPaint(this SKColor skColor)
        => new(skColor);

    /// <summary>
    /// Converts a hexadecimal color string to its ARGB components.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color string to convert. It can include an optional '#' prefix and must represent either an RGB or ARGB color (6 or 8 characters).</param>
    /// <returns>A tuple containing the Alpha, Red, Green, and Blue components as bytes, or null if the input is invalid.</returns>
    public static (byte Alpha, byte Red, byte Green, byte Blue)? ToArgb(this string hexColor)
    {
        if (hexColor.StartsWith('#')) hexColor = hexColor[1..];

        if (hexColor.Length == 6) hexColor = "FF" + hexColor;

        if (hexColor.Length is not 8) return null;

        byte a, r, g, b;

        try
        {
            a = ConvertToByte(hexColor[..2]);
            r = ConvertToByte(hexColor.Substring(2, 2));
            g = ConvertToByte(hexColor.Substring(4, 2));
            b = ConvertToByte(hexColor.Substring(6, 2));
        }
        catch
        {
            return null;
        }

        return (a, r, g, b);
    }

    /// <summary>
    /// Converts a hexadecimal string value to a byte.
    /// </summary>
    /// <param name="hex">The hexadecimal string value to convert.</param>
    /// <returns>The byte representation of the hexadecimal string value.</returns>
    public static byte ConvertToByte(string hex)
        => byte.Parse(hex, NumberStyles.HexNumber);
}