using System.Windows.Media;
using System.Globalization;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MyExpenses.Wpf.Utils;

public static class ColorExtensions
{
    /// <summary>
    /// Converts a System.Drawing.Color object to HSV (Hue, Saturation, Value) values.
    /// </summary>
    /// <param name="color">The System.Drawing.Color object to convert.</param>
    /// <returns>A tuple containing the HSV values, represented as doubles.</returns>
    public static (double Hue, double Saturation, double Value) ToHsv(this Color color)
    {
        double h = 0, s;

        double min = Math.Min(Math.Min(color.R, color.G), color.B);
        double v = Math.Max(Math.Max(color.R, color.G), color.B);
        var delta = v - min;

        if (v == 0.0) s = 0;
        else s = delta / v;

        if (s == 0) h = 0.0;
        else
        {
            const double tolerance = 0.0001;

            if (Math.Abs(color.R - v) < tolerance) h = (color.G - color.B) / delta;
            else if (Math.Abs(color.G - v) < tolerance) h = 2 + (color.B - color.R) / delta;
            else if (Math.Abs(color.B - v) < tolerance) h = 4 + (color.R - color.G) / delta;

            h *= 60;

            if (h < 0.0) h += 360;
        }

        return (h, s, v / 255);
    }

    /// <summary>
    /// Converts a System.Drawing.Color object to a hexadecimal color code.
    /// </summary>
    /// <param name="color">The System.Drawing.Color object to convert.</param>
    /// <returns>The hexadecimal color code representing the converted color as a string.</returns>
    public static string ToHexadecimal(this Color color)
        => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

    /// <summary>
    /// Converts a System.Drawing.Color object to a hexadecimal color code without including the alpha channel.
    /// </summary>
    /// <param name="color">The System.Drawing.Color object to convert.</param>
    /// <returns>The hexadecimal color code representing the converted color without the alpha channel as a string.</returns>
    public static string ToHexadecimalWithoutAlpha(this Color color)
        => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    /// <summary>
    /// Converts HSV values to a System.Drawing.Color object.
    /// </summary>
    /// <param name="hue">The hue value (0-360).</param>
    /// <param name="saturation">The saturation value (0-1).</param>
    /// <param name="value">The value (brightness) value (0-1).</param>
    /// <param name="alpha">The alpha value (0-255). Default is 255.</param>
    /// <returns>The System.Drawing.Color object representing the converted color.</returns>
    public static Color ToColor(double hue, double saturation, double value, byte alpha = 255)
    {
        if (saturation is 0)
        {
            var gray = (byte)(value * 255);
            return Color.FromArgb(alpha, gray, gray, gray);
        }

        var (r, g, b) = ConvertHsvToRgb(hue, saturation, value);

        return Color.FromArgb(alpha, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    /// <summary>
    /// Converts HSV components to RGB components.
    /// </summary>
    /// <param name="hue">The hue value (0-360).</param>
    /// <param name="saturation">The saturation value (0-1).</param>
    /// <param name="value">The brightness value (0-1).</param>
    /// <returns>A tuple representing the RGB components as doubles (range 0-1).</returns>
    private static (double r, double g, double b) ConvertHsvToRgb(double hue, double saturation, double value)
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
        var color = hexColor.ToColor();
        return color?.ToSkColor();
    }

    /// <summary>
    /// Converts a System.Drawing.Color object to a SkiaSharp.SKColor object.
    /// </summary>
    /// <param name="color">The System.Drawing.Color object to convert.</param>
    /// <returns>The SkiaSharp.SKColor object representing the converted color.</returns>
    public static SKColor ToSkColor(this Color color)
        => new(color.R, color.G, color.B, color.A);

    /// <summary>
    /// Converts a System.Drawing.Color object to a SolidColorBrush instance.
    /// </summary>
    /// <param name="color">The System.Drawing.Color object to convert.</param>
    /// <returns>A SolidColorBrush instance representing the converted color.</returns>
    public static SolidColorBrush ToSolidColorBrush(this Color color)
        => new(color);

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
    /// Converts a hexadecimal color code to a System.Drawing.Color object.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color code to convert.</param>
    /// <returns>A System.Drawing.Color object representing the converted color. Returns null if the conversion fails.</returns>
    public static Color? ToColor(this string hexColor)
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

        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    /// Converts a hexadecimal string value to a byte.
    /// </summary>
    /// <param name="hex">The hexadecimal string value to convert.</param>
    /// <returns>The byte representation of the hexadecimal string value.</returns>
    private static byte ConvertToByte(string hex)
        => byte.Parse(hex, NumberStyles.HexNumber);
}