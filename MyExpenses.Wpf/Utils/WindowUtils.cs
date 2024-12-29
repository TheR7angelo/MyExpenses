using System.Windows;
using System.Windows.Interop;
using MyExpenses.Utils.WindowStyle;

namespace MyExpenses.Wpf.Utils;

public static class WindowUtils
{
    public static void SetWindowCornerPreference(this Window window, DwmWindowCornerPreference dwmWindowCornerPreference = DwmWindowCornerPreference.Round)
    {
        var w = Window.GetWindow(window)!;
        var hWnd = new WindowInteropHelper(w).EnsureHandle();
        _ = hWnd.SetWindowCornerPreference(dwmWindowCornerPreference);
    }
}