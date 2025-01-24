using System.Text.RegularExpressions;
using MyExpenses.SharedUtils.Converters;

namespace MyExpenses.SharedUtils.RegexUtils;

public static class FileNameExtractDateTime
{
    public static DateTime? ExtractDateTime(string fileName)
    {
        if (File.Exists(fileName)) fileName = Path.GetFileNameWithoutExtension(fileName);

        var regex = new Regex(@"(?<=_)\d{8}_\d{6}(?=.sqlite)");
        var match = regex.Match(fileName);

        return match.Success ? match.Value.ConvertFromString() : null;
    }
}