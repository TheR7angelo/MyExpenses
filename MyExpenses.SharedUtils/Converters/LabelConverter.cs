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
    public static IEnumerable<string> ToTransformLabelsToTitleCaseDateFormat(this IEnumerable<string> labels)
    {
        foreach (var label in labels)
        {
            var labelSplit = label.Split('-');
            if (!int.TryParse(labelSplit[0], out var year) || !int.TryParse(labelSplit[1], out var month))
                throw new FormatException($"Invalid format for label '{label}'. Expected 'YYYY-MM' format.");

            var d = new DateOnly(year, month, 1);
            var newLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.ToString("MMMM yyyy"));
            yield return newLabel;
        }
    }

    /// <summary>
    /// Converts a collection of label strings in title case format back into a consistent "MMMM yyyy" date format.
    /// </summary>
    /// <param name="labels">The collection of labels to process and convert.</param>
    /// <returns>A collection of strings in "MMMM yyyy" date format.</returns>
    /// <exception cref="FormatException">Thrown when a label can't be parsed into the expected format or culture.</exception>
    private static IEnumerable<string> ToTransformLabelsToTitleCaseDateFormatConvertBack(this IEnumerable<string> labels)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

        var labelsArray = labels.ToArray();
        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var label in labelsArray)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            DateTime dateTime = default;

            // ReSharper disable once HeapView.DelegateAllocation
            var culture = cultures.FirstOrDefault(c => DateTime.TryParseExact(label, "MMMM yyyy", c, DateTimeStyles.None, out dateTime));
            if (culture is null) throw new FormatException($"Invalid format for label '{label}'. Cannot determine the original culture.");

            yield return dateTime.ToString("yyyy-MM");
        }
    }

    /// <summary>
    /// Performs a round-trip transformation of a collection of label strings in "YYYY-MM" date format to a title case format and back.
    /// </summary>
    /// <param name="labels">The collection of label strings to process through the round-trip transformation.</param>
    /// <returns>A collection of strings in title case format after the round-trip transformation.</returns>
    /// <exception cref="FormatException">Thrown when a label doesn't follow the expected format or fails during the transformation process.</exception>
    public static List<string> ToRoundTripDateLabelTransformation(this IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormatConvertBack();
        var results = transformedLabels.ToTransformLabelsToTitleCaseDateFormat().ToList();

        return results;
    }
}