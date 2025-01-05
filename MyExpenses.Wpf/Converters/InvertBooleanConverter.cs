using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class InvertBooleanConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not bool b
            ? value
            : Invert(b);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not bool b
            ? value
            : Invert(b);
    }

    private static bool Invert(bool value)
        => !value;
}