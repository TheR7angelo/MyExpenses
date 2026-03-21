using System.Windows;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Dwm;
using MyExpenses.Utils.WindowStyle;
using Serilog;

namespace MyExpenses.Wpf.Utils
{
    public static class WindowUtils
    {
        /// <summary>
        /// Sets the corner preference for the specified WPF window, enabling support for rounded or other styled corners if supported by the operating system.
        /// </summary>
        /// <param name="window">The WPF window for which the corner preference is being set.</param>
        /// <param name="preference">
        /// The desired corner preference value. Defaults to DWMWCP_ROUND, which enables rounded corners on supported operating systems.
        /// </param>
        public static void SetWindowCornerPreference(this Window window,
            DWM_WINDOW_CORNER_PREFERENCE preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND)
        {
            if (!WindowsVersion.IsWindows11OrLater) return;

            try
            {
                var w = Window.GetWindow(window);
                if (w is null)
                {
                    Log.Error("Window is null");
                    return;
                }

                var hwnd = new WindowInteropHelper(w).EnsureHandle();
                if (hwnd == IntPtr.Zero)
                {
                    Log.Error("Window handle is invalid, skipping SetWindowCornerPreference");
                    return;
                }

                unsafe
                {
                    // Sets the value of non-client rendering attributes for a window.
                    // DWMWA_WINDOW_CORNER_PREFERENCE = 33
                    var hr = PInvoke.DwmSetWindowAttribute(
                        (HWND)hwnd,
                        DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                        &preference,
                        sizeof(DWM_WINDOW_CORNER_PREFERENCE)
                    );

                    if (hr.Failed)
                    {
                        // Log warning if the set worked but returned a failure (e.g., on Windows 10)
                        Log.Warning("DwmSetWindowAttribute failed with HRESULT: {HResult}", hr);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error setting window corner preference");
            }
        }
    }
}