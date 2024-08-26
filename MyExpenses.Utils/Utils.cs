using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
    /// Opens the specified file in the default file explorer on the Windows platform.
    /// </summary>
    /// <param name="file">The path to the file to be opened.</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if the method is called on a platform other than Windows.</exception>
    [SupportedOSPlatform("windows")]
    public static void StartFile(this string file)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo("explorer", file) { UseShellExecute = true });
        }
        else
        {
            throw new PlatformNotSupportedException("This method is not supported on this platform.");
        }
    }
}