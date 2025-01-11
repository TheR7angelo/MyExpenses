using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class ValueSymbolConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        => Utils.Converters.ValueSymbolConverter.Convert(values);

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}