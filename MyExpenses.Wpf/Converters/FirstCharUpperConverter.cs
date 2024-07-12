using System.Globalization;
using System.Windows.Data;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Wpf.Converters;

public class FirstCharUpperConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not string str ? value : str.ToFirstCharUpper();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}