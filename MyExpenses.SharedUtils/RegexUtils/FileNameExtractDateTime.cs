using System.Globalization;
using System.Text.RegularExpressions;
using MyExpenses.SharedUtils.GlobalInfos;

namespace MyExpenses.SharedUtils.RegexUtils;

public static class FileNameExtractDateTime
{
    public static DateTime? ExtractDateTime(this string fileName)
    {
        if (File.Exists(fileName)) fileName = Path.GetFileNameWithoutExtension(fileName);

        var pattern = $@"(?<=_)\d{{8}}_\d{{6}}(?={DatabaseInfos.Extension})";

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This Regex instance is intentionally created with a dynamic pattern.
        var regex = new Regex(pattern, RegexOptions.Compiled);
        var match = regex.Match(fileName);

        if (!match.Success) return null;

        var success = DateTime.TryParseExact(match.Value, DatabaseInfos.FormatDateTimeBackup, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime);
        return success ? dateTime : null;
    }
}