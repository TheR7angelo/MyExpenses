using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace MyExpenses.Utils;

public static class Utils
{
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