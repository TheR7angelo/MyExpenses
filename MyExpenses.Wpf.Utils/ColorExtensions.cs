using System.Drawing;
using System.Globalization;
using SkiaSharp;

namespace MyExpenses.Wpf.Utils;

public static class ColorExtensions
{
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