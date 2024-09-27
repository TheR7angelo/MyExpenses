namespace MyExpenses.Wpf.Resources.Regex;

public static class DecimalRegex
{
    /// <summary>
    /// Checks if the given string contains only decimal numbers.
    /// </summary>
    /// <param name="txt">The string to check.</param>
    /// <returns>True if the string contains only decimal numbers, otherwise false.</returns>
    public static bool IsOnlyDecimal(this string txt)
    {
        var regex = new System.Text.RegularExpressions.Regex("^-?[.][0-9]+$|^-?[0-9]*[.]{0,1}[0-9]*$|^-?[0-9]*[,]{0,1}[0-9]*$");
        var result = !regex.IsMatch(txt);
        return result;
    }
}