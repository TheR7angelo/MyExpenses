namespace MyExpenses.Utils.Strings;

public static class StringsExtensions
{
    /// <summary>
    /// Converts a string to an int value (nullable).
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="output">The converted int value (nullable).</param>
    /// <returns>A boolean indicating if the conversion was successful.</returns>
    public static bool ToInt(this string input, out int? output)
    {
        var success = int.TryParse(input, out var i);

        output = success ? i : null;

        return success;
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