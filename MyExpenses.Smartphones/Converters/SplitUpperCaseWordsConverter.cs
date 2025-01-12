using System.Globalization;
namespace MyExpenses.Smartphones.Converters;

public class SplitUpperCaseWordsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Utils.Converters.SplitUpperCaseWordsConverter.Convert(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}