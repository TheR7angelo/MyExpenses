using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class InvertBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.InvertBooleanConverter.Convert(value);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.InvertBooleanConverter.ConvertBack(value);
}