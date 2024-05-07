using System.Globalization;
using System.Windows.Data;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Converters;

public class ColorToHexConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= "#FF000000";

        if (value is not string str) return Binding.DoNothing;
        if (string.IsNullOrEmpty(str)) str = "#FF000000";

        var color = str.ToColor();

        return color;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}