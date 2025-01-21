using MyExpenses.SharedUtils.RegexUtils;

namespace MyExpenses.Utils.Converters;

public static class SplitUpperCaseWordsConverter
{
    public static object? Convert(object? value)
    {
        value = value?.ToString();
        if (value is not string str || string.IsNullOrEmpty(str)) return value;
        str = str.SplitUpperCaseWord();
        return str;
    }
}