using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // ReSharper disable once HeapView.BoxingAllocation
        if (value is not bool b) return Visibility.Visible;

        // ReSharper disable once HeapView.BoxingAllocation
        return b ? Visibility.Collapsed : Visibility.Visible;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility) return value;

        // ReSharper disable once HeapView.BoxingAllocation
        return visibility is Visibility.Collapsed;
    }
}