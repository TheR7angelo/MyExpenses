using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class RoundThreeDecimalConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.RoundDecimalConverter.Convert(value, 3);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.RoundDecimalConverter.ConvertBack(value);
}