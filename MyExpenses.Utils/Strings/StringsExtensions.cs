namespace MyExpenses.Utils.Strings;

public static class StringsExtensions
{
    /// <summary>
    /// Converts the specified string representation of a number to an equivalent 32-bit signed integer.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The 32-bit signed integer equivalent to the number specified in the string.</returns>
    /// <exception cref="FormatException">The string does not contain a valid representation of a number.</exception>
    public static int ToInt(this string input)
    {
        if (int.TryParse(input, out var i))
        {
            return i;
        }

        throw new FormatException();
    }

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