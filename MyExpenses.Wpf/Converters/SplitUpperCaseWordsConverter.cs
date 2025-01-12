using System.Globalization;
using System.Windows.Data;

namespace MyExpenses.Wpf.Converters;

public class SplitUpperCaseWordsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => MyExpenses.Utils.Converters.SplitUpperCaseWordsConverter.Convert(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}