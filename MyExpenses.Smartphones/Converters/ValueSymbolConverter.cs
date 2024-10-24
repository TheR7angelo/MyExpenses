using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class ValueSymbolConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) values = [0d, string.Empty];
        if (values.Length > 2) values = values[..2];

        values[0] ??= 0d;
        values[1] ??= string.Empty;

        if (!double.TryParse(values[0]?.ToString(), out var numericValue)) numericValue = 0;

        var value = numericValue.ToString("F2");
        var symbol = values[1]?.ToString();

        return !string.IsNullOrWhiteSpace(symbol) ? $"{value} {symbol}" : value;
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}