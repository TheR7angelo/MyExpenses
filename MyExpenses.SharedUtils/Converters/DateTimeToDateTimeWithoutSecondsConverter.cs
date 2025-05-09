using System.Globalization;

namespace MyExpenses.SharedUtils.Converters;

/// <summary>
/// Provides functionality to convert a DateTime object to a representation without seconds.
/// </summary>
public static class DateTimeToDateTimeWithoutSecondsConverter
{
    /// <summary>
    /// Converts a DateTime object to a string representation without seconds.
    /// </summary>
    /// <param name="value">The DateTime object to convert.</param>
    /// <param name="culture">An optional CultureInfo object to specify the culture-specific formatting. If not provided, the current culture is used.</param>
    /// <returns>A string representation of the DateTime object without seconds. Returns an empty string if the input is not a DateTime object.</returns>
    public static string Convert(this object? value, CultureInfo? culture = null)
    {
        if (value is not DateTime dateTime) return string.Empty;

        var cultureToUse = culture ?? CultureInfo.CurrentCulture;

        var datePattern = cultureToUse.DateTimeFormat.ShortDatePattern;
        var timePattern = cultureToUse.DateTimeFormat.ShortTimePattern.Replace(":ss", "");

        return dateTime.ToString($"{datePattern} {timePattern}", cultureToUse);
    }

    /// <summary>
    /// Converts a string representation of a date and time back to a DateTime object or a related representation.
    /// </summary>
    /// <param name="value">The object to be converted back, typically a string representation of a date and time.</param>
    /// <param name="culture">Culture-specific information used for parsing the string.</param>
    /// <returns>
    /// A DateTime object if conversion is successful; otherwise, null if the value cannot be converted.
    /// </returns>
    public static object? ConvertBack(this object? value, CultureInfo culture)
        => value is not string str ? null : str.ConvertFromString(culture);

}