namespace MyExpenses.Utils.Converters;

public static class RoundDecimalConverter
{
    public static object? Convert(object? value, int digit)
    {
        if (value is double doubleValue)
        {
            return Math.Round(doubleValue, digit);
        }

        return value;
    }

    public static object? ConvertBack(object? value)
    {
        if (value is string stringValue && double.TryParse(stringValue, out var doubleValue))
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return doubleValue;
        }

        return value;
    }
}