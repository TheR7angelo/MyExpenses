using System.Globalization;
using Brush = Microsoft.Maui.Controls.Brush;
using Color = Microsoft.Maui.Graphics.Color;

namespace MyExpenses.Smartphones.Converters;

public class StringToSolidColorBrush : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorString || string.IsNullOrWhiteSpace(colorString)) return Brush.Default;
        try
        {
            var color = Color.FromArgb(colorString);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // A new SolidColorBrush instance is explicitly allocated here to create a brush
            // from the given color string. This allocation is necessary to convert the ARGB
            // string into a SolidColorBrush object, which can then be used for UI rendering.
            // The use of a new instance ensures that the resulting brush is specific to the
            // provided color and does not interfere with existing brushes or UI elements.
            var solidColorBrush = new SolidColorBrush(color);
            return solidColorBrush;
        }
        catch (Exception)
        {
            return Brush.Default;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush solidColorBrush) return Binding.DoNothing;
        var color = solidColorBrush.Color;
        return color.ToHex();
    }
}