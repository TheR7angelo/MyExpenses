using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Converters.Colors;

public class HexToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return null;

        if (str.StartsWith('#')) str = str[1..];

        if(str.Length > 8) str = str[..8];

        var color = str.ToColor();
        // ReSharper disable once HeapView.BoxingAllocation
        return color;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not Color color ? null : color.ToHexadecimal();
    }
}