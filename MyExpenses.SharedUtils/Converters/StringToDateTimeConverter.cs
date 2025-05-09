using System.Globalization;

namespace MyExpenses.SharedUtils.Converters;

public static class StringToDateTimeConverter
{
    private static readonly string[] SupportedFormats =
    [
        "dd/MM/yyyy HH:mm",
        "MM/dd/yyyy HH:mm",
        "yyyy/MM/dd HH:mm:ss.fff",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "MM-dd-yyyy HH:mm",
        "yyyy-MM-dd HH:mm:ss.fff",
        "yyyy-MM-dd HH:mm:ss"
    ];

    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Converts a string representation of a date and time to a nullable DateTime object
    /// using predefined formats and an optional culture.
    /// </summary>
    /// <param name="input">The string representation of the date and time to be converted.</param>
    /// <param name="cultureInfo">Optional. The culture information used for the conversion. If null, the invariant culture is used.</param>
    /// <returns>
    /// A nullable DateTime object representing the converted value if the string matches one of the supported formats; otherwise, null.
    /// </returns>
    public static DateTime? ConvertFromString(this string? input, CultureInfo? cultureInfo = null)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var culture = cultureInfo ?? InvariantCulture;
        if (DateTime.TryParseExact(
                input,
                SupportedFormats,
                culture,
                DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        return null;
    }
}