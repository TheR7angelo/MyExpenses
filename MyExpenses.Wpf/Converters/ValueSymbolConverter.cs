using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class ValueSymbolConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = ((double)values[0]).ToString("F2");
        var symbol = values[1].ToString() ?? "";
        return $"{value} {symbol}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}