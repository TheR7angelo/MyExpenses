using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class ValueToCreditDebitedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.ValueToCreditDebitedConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return Binding.DoNothing;
    }
}