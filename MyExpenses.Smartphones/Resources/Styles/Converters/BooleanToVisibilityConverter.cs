using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        // ReSharper disable once HeapView.BoxingAllocation
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return boolValue;
        }
        // ReSharper disable once HeapView.BoxingAllocation
        return false;
    }
}