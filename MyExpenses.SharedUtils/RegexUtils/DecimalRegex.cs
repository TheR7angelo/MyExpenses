using System.Text.RegularExpressions;

namespace MyExpenses.SharedUtils.RegexUtils;

public static partial class DecimalRegex
{
    /// <summary>
    /// Checks if the given string contains only decimal numbers.
    /// </summary>
    /// <param name="txt">The string to check.</param>
    /// <returns>True if the string contains only decimal numbers, otherwise false.</returns>
    public static bool IsOnlyDecimal(this string txt)
    {
        var result = !IsOnlyDecimalRegex().IsMatch(txt);
        return result;
    }

    [GeneratedRegex("^-?[.][0-9]+$|^-?[0-9]*[.]{0,1}[0-9]*$|^-?[0-9]*[,]{0,1}[0-9]*$")]
    private static partial Regex IsOnlyDecimalRegex();
}