using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;

namespace MyExpenses.Wpf.Converters.Colors;

public class SolidColorBrushToSolidColorBrushContrastingForegroundColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not SolidColorBrush solidColorBrush
            ? null
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            : new SolidColorBrush(solidColorBrush.Color.ContrastingForegroundColor());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}