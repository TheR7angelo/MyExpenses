using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace MyExpenses.Wpf.Converters;

/// <summary>
/// Converts a total value to an icon from the MaterialDesignThemes.Wpf library.
/// </summary>
public class TotalToIconConverter : IValueConverter
{
    /// <summary>
    /// Converts a total value to an icon from the MaterialDesignThemes.Wpf library.
    /// </summary>
    /// <param name="value">The total value to be converted.</param>
    /// <param name="targetType">The type of the target property.</param>
    /// <param name="parameter">An optional parameter passed to the converter.</param>
    /// <param name="culture">The culture to be used for conversion.</param>
    /// <returns>The icon from the MaterialDesignThemes.Wpf library corresponding to the total value.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value ??= 0d;
        if (value is not double total) return PackIconKind.None;
        return total switch
        {
            < 0 => PackIconKind.WeatherPouring,
            0 => PackIconKind.WeatherPartlyCloudy,
            _ => PackIconKind.WhiteBalanceSunny
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // No return
        return null;
    }
}