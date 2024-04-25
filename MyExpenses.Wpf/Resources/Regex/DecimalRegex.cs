using System.Text.RegularExpressions;

namespace MyExpenses.Wpf.Resources.Regex;

public static partial class DecimalRegex
{
    public static bool IsOnlyDecimal(this string txt)
    {
        var regex = IsOnlyDecimalRegex();
        var result = !regex.IsMatch(txt);
        return result;
    }

    [GeneratedRegex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$|^[0-9]*[,]{0,1}[0-9]*$")]
    private static partial System.Text.RegularExpressions.Regex IsOnlyDecimalRegex();
}