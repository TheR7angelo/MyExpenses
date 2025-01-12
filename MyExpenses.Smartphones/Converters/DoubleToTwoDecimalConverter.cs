using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class DoubleToTwoDecimalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.DoubleToTwoDecimalConverter.Convert(value, culture);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.DoubleToTwoDecimalConverter.ConvertBack(value);
}