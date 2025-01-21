namespace MyExpenses.SharedUtils.RegexUtils;

public static class StringRegex
{
    /// <summary>
    /// Splits a string into separate words based on uppercase letters.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <returns>The input string with spaces inserted between each word.</returns>
    public static string SplitUpperCaseWord(this string str)
    {
        var regex = new System.Text.RegularExpressions.Regex("([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))");
        str = regex.Replace(str, "$1 ");
        return str;
    }
}