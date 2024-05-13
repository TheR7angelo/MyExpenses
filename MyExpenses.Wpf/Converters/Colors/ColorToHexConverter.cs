using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Converters.Colors;

public class ColorToHexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not Color color ? null : color.ToHexadecimal();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return null;
        if (str.StartsWith('#')) str = str[1..];

        switch (str.Length)
        {
            case < 8:
                return null;
            case > 8:
                str = str[..8];
                break;
        }

        var color = str.ToColor();
        return color;
    }
}