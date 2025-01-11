using System.Globalization;
using MyExpenses.Utils.Converters;

namespace MyExpenses.Smartphones.Converters;

public class InvertBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value.Convert();
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value.ConvertBack();
}