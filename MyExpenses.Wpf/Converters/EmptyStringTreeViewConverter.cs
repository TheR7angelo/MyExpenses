using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class EmptyStringTreeViewConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.EmptyStringTreeViewConverter.Convert(value);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public static string ToUnknown(object? value = null)
        => MyExpenses.Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(value);
}