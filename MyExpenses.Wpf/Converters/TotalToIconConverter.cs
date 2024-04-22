using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace MyExpenses.Wpf.Converters;

public class TotalToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double total) return PackIconKind.None; // Return a default icon or null
        return total switch
        {
            < 0 => PackIconKind.WeatherPouring,
            0 => PackIconKind.WeatherPartlyCloudy,
            _ => PackIconKind.WhiteBalanceSunny
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}