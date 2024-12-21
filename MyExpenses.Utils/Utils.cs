using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyExpenses.Utils;

public static class Utils
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

    /// <summary>
    /// Opens the MyExpenses GitHub page.
    /// </summary>
    /// <remarks>
    /// This method opens the web page for the MyExpenses project on GitHub.
    /// The URL for the page is "https://github.com/TheR7angelo/MyExpenses".
    /// </remarks>
    /// <seealso cref="StartProcess(string)"/>
    public static void OpenGithubPage()
    {
        const string url = "https://github.com/TheR7angelo/MyExpenses";
        url.StartProcess();
    }

    /// <summary>
    /// Opens the specified process.
    /// </summary>
    /// <param name="process">The path or name of the process to be opened.</param>
    /// <remarks>
    /// This method opens the specified process using the default program associated with it.
    /// </remarks>
    public static void StartProcess(this string process)
    {
        Process.Start(new ProcessStartInfo(process) { UseShellExecute = true });
    }

    /// <summary>
    /// Opens the specified file or folder in the default system file explorer or viewer.
    /// </summary>
    /// <param name="file">The path of the file or folder to open.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified file or folder does not exist.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when the operation is not supported on the current platform.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file or folder cannot be opened due to an unexpected error.</exception>
    public static void StartFile(this string file)
    {
        if (!File.Exists(file)) throw new FileNotFoundException($"The file '{file}' does not exist.");

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("explorer", file) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", file);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", file);
            }
            else
            {
                throw new PlatformNotSupportedException($"This platform ({RuntimeInformation.OSDescription}) is not supported.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to open the file.", ex);
        }
    }
}