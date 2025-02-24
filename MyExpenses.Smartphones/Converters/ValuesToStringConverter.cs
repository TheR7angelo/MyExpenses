using System.Globalization;
using MyExpenses.SharedUtils.Converters;

namespace MyExpenses.Smartphones.Converters;

public class ValuesToStringConverter : IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
        => values.Convert(culture);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}