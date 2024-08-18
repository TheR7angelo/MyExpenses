using System.Globalization;
using System.Windows.Data;
using MyExpenses.Utils.Dates;

namespace MyExpenses.Wpf.Converters;

public class DateOnlyToDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
        {
            return DateExtensions.ToDateTime(dateOnly);
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return DateExtensions.ToDateOnly(dateTime);
        }
        return value;
    }
}