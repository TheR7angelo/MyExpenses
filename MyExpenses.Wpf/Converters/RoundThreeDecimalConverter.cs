using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class RoundThreeDecimalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return Math.Round(doubleValue, 3);
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && double.TryParse(stringValue, out var doubleValue))
        {
            return doubleValue;
        }

        return value;
    }
}