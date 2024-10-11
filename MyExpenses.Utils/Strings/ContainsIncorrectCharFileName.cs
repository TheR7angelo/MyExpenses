namespace MyExpenses.Utils.Strings;

public static class ContainsIncorrectCharFileName
{
    public static bool CheckFilenameContainsIncorrectChar(this string filePath)
    {
        var fileName = File.Exists(filePath)
            ? Path.GetFileName(filePath)
            : filePath;

        if (fileName.StartsWith('.')) return true;
        var charsIncorrects = new[] { '/', '\\', '?', '%', '*', ':', '|', '"', '<', '>', '\0' };
        return charsIncorrects.Any(fileName.Contains);
    }
}