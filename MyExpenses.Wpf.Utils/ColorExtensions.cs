using System.Windows.Media;
using MyExpenses.Utils;
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

        var (r, g, b) = MyExpenses.Utils.ColorExtensions.ConvertHsvToRgb(hue, saturation, value);

        return Color.FromArgb(alpha, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
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
    /// Converts a hexadecimal color code to a System.Drawing.Color object.
    /// </summary>
    /// <param name="hexColor">The hexadecimal color code to convert.</param>
    /// <returns>A System.Drawing.Color object representing the converted color. Returns null if the conversion fails.</returns>
    public static Color? ToColor(this string hexColor)
    {
        if (hexColor.StartsWith('#')) hexColor = hexColor[1..];

        if (hexColor.Length == 6) hexColor = "FF" + hexColor;

        if (hexColor.Length is not 8) return null;

        var convert = hexColor.ToArgb();
        if (!convert.HasValue) return null;

        return Color.FromArgb(convert.Value.Alpha, convert.Value.Red, convert.Value.Green, convert.Value.Blue);
    }
}