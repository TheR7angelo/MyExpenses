using System.Globalization;
using MyExpenses.Sql.Utils.Regex;

namespace MyExpenses.Smartphones.Converters;

public class SplitUpperCaseWordsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        value = value?.ToString();
        if (value is not string str || string.IsNullOrEmpty(str)) return value;
        str = str.SplitUpperCaseWord();
        return str;

    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}