namespace MyExpenses.Utils.Strings;

public static class StringsExtensions
{
    /// <summary>
    /// Converts the first character of a string to uppercase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input string with the first character in uppercase.</returns>
    public static string ToFirstCharUpper(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input[1..];
    }
}