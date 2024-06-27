using System.Runtime.InteropServices;

namespace MyExpenses.Utils.WindowStyle;

public static class WindowAttribute
{
    // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
    [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern void DwmSetWindowAttribute(this IntPtr hwnd,
        DwmWindowAttribute attribute,
        ref DwmWindowCornerPreference pvAttribute,
        uint cbAttribute);
}