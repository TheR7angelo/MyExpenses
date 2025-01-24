namespace MyExpenses.Utils.Strings;

public static class ContainsIncorrectCharFileName
{
    public static bool CheckFilenameContainsIncorrectChar(this string filePath)
    {
        var fileName = File.Exists(filePath)
            ? Path.GetFileName(filePath)
            : filePath;

        if (fileName.StartsWith('.')) return true;

        ReadOnlySpan<char> charsIncorrects = ['/', '\\', '?', '%', '*', ':', '|', '"', '<', '>', '\0'];

        foreach (var c in fileName)
        {
            if (charsIncorrects.Contains(c)) return true;
        }

        return false;

    }
}