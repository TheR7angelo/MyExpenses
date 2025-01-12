using System.Globalization;

namespace MyExpenses.Utils.Converters;

public static class DoubleToTwoDecimalConverter
{
    public static object? Convert(object? value, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return doubleValue.ToString("F2", culture);
        }
        return value;
    }

    public static object? ConvertBack(object? value)
        => value;
}