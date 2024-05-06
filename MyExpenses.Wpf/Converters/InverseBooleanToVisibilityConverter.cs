using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b) return Visibility.Visible;

        return b ? Visibility.Collapsed : Visibility.Visible;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // pass
        return null;
    }
}