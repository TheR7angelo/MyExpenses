using System.Globalization;
using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.Converters;

public class TotalToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= 0d;
        if (value is not double total)
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return EPackIcons.Abacus;
        }

        var icon =  total switch
        {
            < 0 => EPackIcons.WeatherPouring,
            0 => EPackIcons.WeatherPartlyCloudy,
            _ => EPackIcons.WhiteBalanceSunny
        };

        return icon.ToGeometry();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}