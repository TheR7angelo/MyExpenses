using System.Globalization;

namespace MyExpenses.SharedUtils.Converters;

public static class LabelConverter
{
    /// <summary>
    /// Transforms a collection of label strings in "YYYY-MM" date format into a title case format based on the current culture's date representation.
    /// </summary>
    /// <param name="labels">The collection of label strings to process and transform.</param>
    /// <returns>A collection of strings in title case format representing the date.</returns>
    /// <exception cref="FormatException">Thrown when a label doesn't follow the expected "YYYY-MM" format or can't be parsed.</exception>
    public static List<string> ToTransformLabelsToTitleCaseDateFormat(this IEnumerable<string> labels)
    {
        var transformedLabels = labels.Select(label =>
        {
            var labelSplit = label.Split('-');

            if (!int.TryParse(labelSplit[0], out var year) || !int.TryParse(labelSplit[1], out var month))
                throw new FormatException($"Invalid format for label '{label}'. Expected 'YYYY-MM' format.");

            var d = new DateOnly(year, month, 1);
            var newLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.ToString("Y"));

            return newLabel;
        }).ToList();

        return transformedLabels;
    }

    /// <summary>
    /// Converts a collection of label strings in title case format back into a consistent "MMMM yyyy" date format.
    /// </summary>
    /// <param name="labels">The collection of labels to process and convert.</param>
    /// <returns>A collection of strings in "MMMM yyyy" date format.</returns>
    /// <exception cref="FormatException">Thrown when a label can't be parsed into the expected format or culture.</exception>
    public static IEnumerable<string> ToTransformLabelsToTitleCaseDateFormatConvertBack(this IEnumerable<string> labels)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

        foreach (var label in labels)
        {
            DateTime dateTime = default;

            // ReSharper disable once HeapView.DelegateAllocation
            var culture = cultures.FirstOrDefault(c => DateTime.TryParseExact(label, "MMMM yyyy", c, DateTimeStyles.None, out dateTime));
            if (culture is null) throw new FormatException($"Invalid format for label '{label}'. Cannot determine the original culture.");

            yield return dateTime.ToString("yyyy-MM");
        }
    }
}