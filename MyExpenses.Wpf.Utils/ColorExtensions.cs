using System.Drawing;
using System.Globalization;
using SkiaSharp;

namespace MyExpenses.Wpf.Utils;

public static class ColorExtensions
{
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
        double r, g, b;
        if (saturation == 0)
        {
            r = value;
            g = value;
            b = value;
        }
        else
        {
            const double tolerance = 0.00001;
            if (Math.Abs(hue - 360.0) < tolerance) hue = 0;
            else hue = hue / 60;

            var i = (int)Math.Truncate(hue);
            var f = hue - i;

            var p = value * (1.0 - saturation);
            var q = value * (1.0 - saturation * f);
            var t = value * (1.0 - saturation * (1.0 - f));

            switch (i)
            {
                case 0:
                    r = value;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = value;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = value;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = value;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = value;
                    break;
                default:
                    r = value;
                    g = p;
                    b = q;
                    break;
            }
        }

        return Color.FromArgb(alpha, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
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
    /// Converts a hexadecimal color code to a System.Drawing.Color object.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color code to convert.</param>
    /// <returns>A System.Drawing.Color object representing the converted color. Returns null if the conversion fails.</returns>
    public static Color? ToColor(this string hexColor)
    {
        if (hexColor.StartsWith('#')) hexColor = hexColor[1..];

        if (hexColor.Length == 6) hexColor = "FF" + hexColor;

        if (hexColor.Length != 8) return null;

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