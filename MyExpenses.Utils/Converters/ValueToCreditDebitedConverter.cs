using MyExpenses.Utils.Resources.Resx.Converters.ValueToCreditDebitedConverter;

namespace MyExpenses.Utils.Converters;

public static class ValueToCreditDebitedConverter
{
    public static object? Convert(object? value)
    {
        if (value is not double doubleValue) return null;

        return doubleValue >= 0
            ? ValueToCreditDebitedConverterResources.CreditedOn
            : ValueToCreditDebitedConverterResources.DebitedOn;
    }
}