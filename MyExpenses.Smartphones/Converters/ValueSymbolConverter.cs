using System.Globalization;
using MyExpenses.Utils.Converters;

namespace MyExpenses.Smartphones.Converters;

public class ValueSymbolConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        => values.Convert();

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}