using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class MaxLengthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.MaxLengthConverter.Convert(value, parameter);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}