namespace MyExpenses.Utils.Converters;

public static class InvertBooleanConverter
{
    public static object? Convert(this object? value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        return value is not bool b
            ? value
            : Invert(b);
    }

    public static object? ConvertBack(this object? value)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        return value is not bool b
            ? value
            : Invert(b);
    }

    private static bool Invert(bool value)
        => !value;
}