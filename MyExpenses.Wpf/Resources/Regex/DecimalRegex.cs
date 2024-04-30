using System.Text.RegularExpressions;

namespace MyExpenses.Wpf.Resources.Regex;

public static partial class DecimalRegex
{
    /// <summary>
    /// Checks if the given string contains only decimal numbers.
    /// </summary>
    /// <param name="txt">The string to check.</param>
    /// <returns>True if the string contains only decimal numbers, otherwise false.</returns>
    public static bool IsOnlyDecimal(this string txt)
    {
        var regex = IsOnlyDecimalRegex();
        var result = !regex.IsMatch(txt);
        return result;
    }

    [GeneratedRegex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$|^[0-9]*[,]{0,1}[0-9]*$")]
    private static partial System.Text.RegularExpressions.Regex IsOnlyDecimalRegex();
}