using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MyExpenses.Utils;

public static class Utils
{
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