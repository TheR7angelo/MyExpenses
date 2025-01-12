using System.Globalization;
using MyExpenses.Utils.Resources.Resx.Converters.DateOnlyToStringConverter;

namespace MyExpenses.Utils.Converters;

public static class DateOnlyToStringConverter
{
    public static object? Convert(object? value)
    {
        var format = DateOnlyToStringConverterResources.DateFormat;

        if (value is DateOnly dateOnly) return dateOnly.ToString(format);

        return value;
    }

    public static object? ConvertBack(object? value, CultureInfo culture)
    {
        if (value is not string dateString) return value;
        return DateTime.TryParse(dateString, culture, DateTimeStyles.None, out var result) ? result : value;
    }
}