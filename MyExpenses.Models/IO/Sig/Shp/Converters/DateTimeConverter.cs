namespace GeoReso.Models.IO.Shape.Converters;

public class DateTimeConverter
{
    private readonly string[] _formats =
        { "dd/MM/yyyy HH:mm", "MM/dd/yyyy HH:mm", "yyyy/MM/dd HH:mm:ss.fff", "yyyy/MM/dd HH:mm:ss",
            "yyyy-MM-dd HH:mm", "MM-dd-yyyy HH:mm", "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss" };

    public DateTime? ConvertFromString(string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;

        if (DateTime.TryParseExact(text, _formats, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var result))
        {
            return result;
        }

        return null;
    }
}