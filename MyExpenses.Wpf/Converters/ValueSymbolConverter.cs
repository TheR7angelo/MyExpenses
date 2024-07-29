using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class ValueSymbolConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return Binding.DoNothing;
        if (values[0] is null || values[1] is null) return Binding.DoNothing;

        if (!double.TryParse(values[0]?.ToString(), out var numericValue)) return Binding.DoNothing;

        var value = numericValue.ToString("F2");
        var symbol = values[1]?.ToString();

        return !string.IsNullOrWhiteSpace(symbol) ? $"{value} {symbol}" : value;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        //pass
        return [];
    }
}