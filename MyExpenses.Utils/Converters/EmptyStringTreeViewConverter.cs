using MyExpenses.Utils.Resources.Resx.Converters;

namespace MyExpenses.Utils.Converters;

public static class EmptyStringTreeViewConverter
{
    public static string Convert(object? value)
        => ToUnknown(value);

    public static string ToUnknown(object? value)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str)
            ? str
            : EmptyStringTreeViewConverterResources.Unknown;
    }
}