using System.Globalization;
using MyExpenses.Smartphones.ColorManipulation;

namespace MyExpenses.Smartphones.Resources.Styles.Converters;

public class SolidColorBrushToSolidColorBrushContrastingForegroundColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not SolidColorBrush solidColorBrush ? null : new SolidColorBrush(solidColorBrush.Color.ContrastingForegroundColor());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}