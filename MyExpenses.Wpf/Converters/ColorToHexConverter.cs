using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Converters;

public class ColorToHexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not Color color ? null : color.ToHexadecimal();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not string str ? Colors.Black : str.ToColor();
    }
}