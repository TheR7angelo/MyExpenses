using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class BooleanToInverseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return !boolValue;
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return !boolValue;
        }
        return value;
    }
}