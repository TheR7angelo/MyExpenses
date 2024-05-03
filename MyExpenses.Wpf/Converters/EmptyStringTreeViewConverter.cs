using System.Globalization;
using System.Windows.Data;
using MyExpenses.Wpf.Resources.Resx.Converters;

namespace MyExpenses.Wpf.Converters;

public class EmptyStringTreeViewConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str) ? str : EmptyStringTreeViewConverterResources.Unknown;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}