using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class DateOnlyToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.DateOnlyToStringConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.DateOnlyToStringConverter.ConvertBack(value, culture);
}