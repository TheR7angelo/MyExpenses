using System.Windows;
using System.Windows.Interop;
using MyExpenses.Utils.WindowStyle;
using Serilog;

namespace MyExpenses.Wpf.Utils;

public static class WindowUtils
{
    public static void SetWindowCornerPreference(this Window window, DwmWindowCornerPreference dwmWindowCornerPreference = DwmWindowCornerPreference.Round)
    {
        try
        {
            var w = Window.GetWindow(window);
            if (w is null)
            {
                Log.Error("Window is null");
                return;
            };

            var hwd = new WindowInteropHelper(w).EnsureHandle();
            if (hwd == IntPtr.Zero)
            {
                Log.Error("Window handle is invalid, skipping SetWindowCornerPreference");
                return;
            }

            _ = hwd.SetWindowCornerPreference(dwmWindowCornerPreference);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error setting window corner preference");
        }
    }
}