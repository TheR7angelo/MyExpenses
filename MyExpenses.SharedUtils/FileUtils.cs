namespace MyExpenses.SharedUtils;

public static class FileUtils
{
    /// <summary>
    /// Asynchronously copies an existing file to a new file. Overwriting a file of the same name is optionally allowed.
    /// </summary>
    /// <param name="sourceFileName">The path of the file to copy.</param>
    /// <param name="destFileName">The path of the destination file.</param>
    /// <param name="overwrite">A boolean value indicating whether to overwrite the destination file if it already exists.</param>
    /// <returns>A task representing the asynchronous copy operation.</returns>
    // ReSharper disable once HeapView.ClosureAllocation
    public static async Task CopyAsync(this string sourceFileName, string destFileName, bool overwrite = false)
        // ReSharper disable once HeapView.DelegateAllocation
        => await Task.Run(() => File.Copy(sourceFileName, destFileName, overwrite));
}