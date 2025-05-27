using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class ColorToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color) return "#000000";
        var withAlpha = parameter switch
        {
            string paramString when bool.TryParse(paramString, out var parsedBool) => parsedBool,
            bool boolParam => boolParam,
            _ => false
        };

        return color.ToArgbHex(withAlpha);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string hexString || string.IsNullOrWhiteSpace(hexString)) return Colors.Transparent;

        try
        {
            return Color.Parse(hexString);
        }
        catch
        {
            return Colors.Transparent;
        }
    }
}