using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class DateTimeToDateOnlyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
        {
            return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day);
        }
        throw new NotImplementedException();
    }
}