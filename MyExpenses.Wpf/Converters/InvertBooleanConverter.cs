using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class InvertBooleanConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.InvertBooleanConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.InvertBooleanConverter.ConvertBack(value);
}