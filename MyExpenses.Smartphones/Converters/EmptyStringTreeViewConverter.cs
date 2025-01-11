using System.Globalization;

namespace MyExpenses.Smartphones.Converters;

public class EmptyStringTreeViewConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.EmptyStringTreeViewConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public static string ToUnknown(object? value = null)
        => Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(value);
}