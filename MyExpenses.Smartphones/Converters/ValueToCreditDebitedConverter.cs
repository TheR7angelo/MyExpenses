using System.Globalization;
using MyExpenses.Smartphones.Resources.Resx.Converters.ValueToCreditDebitedConverter;

namespace MyExpenses.Smartphones.Converters;

public class ValueToCreditDebitedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double doubleValue) return null;

        return doubleValue >= 0
            ? ValueToCreditDebitedConverterResources.CreditedOn
            : ValueToCreditDebitedConverterResources.DebitedOn;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return Binding.DoNothing;
    }
}