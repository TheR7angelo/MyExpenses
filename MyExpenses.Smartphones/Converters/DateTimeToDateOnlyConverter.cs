using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class DateTimeToDateOnlyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.DateOnlyToDateTimeConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.DateOnlyToDateTimeConverter.ConvertBack(value);
}