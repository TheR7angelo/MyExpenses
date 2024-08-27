using System.Globalization;
using System.Windows.Data;
using MyExpenses.Wpf.Resources.Resx.Converters.DateOnlyToStringConverter;

namespace MyExpenses.Wpf.Converters;

public class DateOnlyToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var format = DateOnlyToStringConverterResources.DateFormat;

        if (value is DateOnly dateOnly) return dateOnly.ToString(format);

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string dateString) return value;
        return DateTime.TryParse(dateString, culture, DateTimeStyles.None, out var result) ? result : value;
    }
}