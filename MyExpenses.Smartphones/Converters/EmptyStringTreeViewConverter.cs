using System.Globalization;
using MyExpenses.Smartphones.Resources.Resx.Converters.EmptyStringTreeViewConverter;

namespace MyExpenses.Smartphones.Converters;

public class EmptyStringTreeViewConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ToUnknown(value);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public static string ToUnknown(object? value = null)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str)
            ? str
            : EmptyStringTreeViewConverterResources.Unknown;
    }
}