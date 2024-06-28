using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MyExpenses.Utils.WindowStyle;

public static partial class DesktopWindowsManager
{
    private const string DwmApiDll = "dwmapi.dll";

    /// <summary>
    /// Retrieve the WindowCornerPreference for the specified window
    /// </summary>
    /// <param name="hWnd">IntPtr with the hWnd</param>
    /// <param name="windowCornerPreference">DwmWindowCornerPreference</param>
    /// <returns>bool true if the set worked</returns>
    public static bool SetWindowCornerPreference(this IntPtr hWnd, DwmWindowCornerPreference windowCornerPreference)
    {
        if (!WindowsVersion.IsWindows11OrLater)
        {
            return false;
        }

        IntPtr refToWindowCornerPreference;
        unsafe
        {
            refToWindowCornerPreference = new IntPtr(Unsafe.AsPointer(ref windowCornerPreference));
        }
        var result = DwmSetWindowAttribute(hWnd, DwmWindowAttributes.WindowCornerPreference, refToWindowCornerPreference, Marshal.SizeOf(typeof(uint)));
        return result.Succeeded();
    }

    /// <summary>
    ///     Sets the value of non-client rendering attributes for a window.
    ///     See
    ///     <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa969524(v=vs.85).aspx">
    ///         DwmSetWindowAttribute
    ///         function
    ///     </a>

    /// </summary>
    /// <param name="hWnd">IntPtr with the handle to the window that will receive the attributes.</param>
    /// <param name="dwAttributeToSet">
    ///     A single DWMWINDOWATTRIBUTE flag to apply to the window. This parameter specifies the
    ///     attribute and the pvAttribute parameter points to the value of that attribute.
    /// </param>
    /// <param name="pvAttributeValue">
    ///     A pointer to the value of the attribute specified in the dwAttribute parameter.
    ///     Different DWMWINDOWATTRIBUTE flags require different value types.
    /// </param>
    /// <param name="cbAttribute">The size, in bytes, of the value type pointed to by the pvAttribute parameter.</param>
    /// <returns></returns>
    [LibraryImport(DwmApiDll, SetLastError = true)]
    private static partial HResult DwmSetWindowAttribute(IntPtr hWnd, DwmWindowAttributes dwAttributeToSet, IntPtr pvAttributeValue, int cbAttribute);
}