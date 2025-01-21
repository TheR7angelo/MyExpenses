namespace MyExpenses.SharedUtils.Utils;

public static class PathUtils
{
    /// <summary>
    /// Retrieves the parent directory of the specified path.
    /// </summary>
    /// <param name="path">The path for which the parent directory will be retrieved.</param>
    /// <param name="depth">The number of levels up in the directory structure that will be traversed.
    /// Default is 1.</param>
    /// <returns>The parent directory of the specified path.</returns>
    public static string GetParentDirectory(this string path, int depth = 1)
    {
        for (var i = 1; i <= depth; i++)
        {
            path = Path.GetDirectoryName(path)!;
        }

        return path;
    }
}