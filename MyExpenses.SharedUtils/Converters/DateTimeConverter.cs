namespace MyExpenses.SharedUtils.Converters;

public class DateTimeConverter
{
    private readonly string[] _supportedFormats =
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

    private static readonly System.Globalization.CultureInfo InvariantCulture =
        System.Globalization.CultureInfo.InvariantCulture;

    /// <summary>
    /// Converts a string to a nullable DateTime, using predefined formats.
    /// </summary>
    /// <param name="input">The string representation of a date and time.</param>
    /// <returns>
    /// A nullable <see cref="DateTime"/> object if the conversion is successful; otherwise, <c>null</c>.
    /// </returns>
    public DateTime? ConvertFromString(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (DateTime.TryParseExact(
                input,
                _supportedFormats,
                InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result))
        {
            return result;
        }

        return null;
    }
}