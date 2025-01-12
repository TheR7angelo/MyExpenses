using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class MaxLengthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.MaxLengthConverter.Convert(value, parameter);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}