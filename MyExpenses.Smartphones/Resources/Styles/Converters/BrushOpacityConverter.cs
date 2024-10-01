using System.Globalization;

namespace MyExpenses.Smartphones.Resources.Styles.Converters;

public class BrushOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color) return null;
        var opacity = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
        var newColor = color.WithAlpha((float)opacity);
        return newColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}