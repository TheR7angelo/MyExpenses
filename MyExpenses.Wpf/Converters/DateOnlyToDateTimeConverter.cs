using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class DateOnlyToDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.DateOnlyToDateTimeConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.DateOnlyToDateTimeConverter.ConvertBack(value);
}