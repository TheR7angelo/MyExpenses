using System.Globalization;
using Brush = Microsoft.Maui.Controls.Brush;
using Color = Microsoft.Maui.Graphics.Color;

namespace MyExpenses.Smartphones.Converters;

public class StringToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorString || string.IsNullOrWhiteSpace(colorString)) return Brush.Default;
        try
        {
            var solidColorBrush = new SolidColorBrush(Color.FromArgb(colorString));
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