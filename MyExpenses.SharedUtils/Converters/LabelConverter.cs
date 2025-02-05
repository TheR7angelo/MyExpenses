using System.Globalization;

namespace MyExpenses.SharedUtils.Converters;

public static class LabelConverter
{
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

    public static List<string> ToTransformLabelsToTitleCaseDateFormatConvertBack(this IEnumerable<string> labels)
    {
        var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

        var transformedLabels = labels.Select(label =>
        {
            DateTime dateTime = default;
            var culture = cultures.FirstOrDefault(c => DateTime.TryParseExact(label, "MMMM yyyy", c, DateTimeStyles.None, out dateTime));

            if (culture is null) throw new FormatException($"Invalid format for label '{label}'. Cannot determine the original culture.");

            return dateTime.ToString("yyyy-MM");
        }).ToList();

        return transformedLabels;
    }
}