using System.Globalization;
using MyExpenses.Smartphones.PackIcons;

namespace MyExpenses.Smartphones.Converters;

public class TotalToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= 0d;
        if (value is not double total) return EPackIcons.Abacus;
        return total switch
        {
            < 0 => EPackIcons.WeatherPouring,
            0 => EPackIcons.WeatherPartlyCloudy,
            _ => EPackIcons.WhiteBalanceSunny
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}