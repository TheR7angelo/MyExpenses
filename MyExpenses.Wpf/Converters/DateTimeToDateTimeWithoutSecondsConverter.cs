using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class DateTimeToDateTimeWithoutSecondsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => SharedUtils.Converters.DateTimeToDateTimeWithoutSecondsConverter.Convert(value, culture);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => SharedUtils.Converters.DateTimeToDateTimeWithoutSecondsConverter.ConvertBack(value, culture);
}