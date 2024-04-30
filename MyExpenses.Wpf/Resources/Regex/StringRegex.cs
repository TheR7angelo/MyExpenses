using System.Text.RegularExpressions;

namespace MyExpenses.Wpf.Resources.Regex;

public static partial class StringRegex
{
    /// <summary>
    /// Splits a string into separate words based on uppercase letters.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <returns>The input string with spaces inserted between each word.</returns>
    public static string SplitUpperCaseWord(this string str)
    {
        str = SplitUpperCaseWordRegex().Replace(str, "$1 ");
        return str;
    }

    [GeneratedRegex("([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))")]
    private static partial System.Text.RegularExpressions.Regex SplitUpperCaseWordRegex();
}