#if ANDROID
using Android.Content.PM;
#elif IOS
using UIKit;
using Foundation;
#endif

namespace MyExpenses.Smartphones;

public class DeviceOrientationService
{
    #if ANDROID

    private static ScreenOrientation ConvertToUiInterfaceOrientation(DisplayOrientation orientation)
    {
        return orientation switch
        {
            DisplayOrientation.Portrait => ScreenOrientation.Portrait,
            DisplayOrientation.Landscape => ScreenOrientation.Landscape,
            _ => ScreenOrientation.Unspecified
        };
    }

    /// <summary>
    /// Sets the device orientation to the specified display orientation.
    /// </summary>
    /// <param name="orientation">The desired display orientation.</param>
    public void SetDeviceOrientation(DisplayOrientation orientation)
    {
        var currentActivity = ActivityStateManager.Default.GetCurrentActivity();
        if (currentActivity is null) return;

        var screenOrientation = ConvertToUiInterfaceOrientation(orientation);
        currentActivity.RequestedOrientation = screenOrientation;
    }

    #elif IOS

    /// <summary>
    /// Sets the orientation of the device to the specified orientation.
    /// </summary>
    /// <param name="orientation">The desired device display orientation.</param>
    public void SetDeviceOrientation(DisplayOrientation orientation)
    {
        var uiOrientation = ConvertToUiInterfaceOrientation(orientation);
        UIDevice.CurrentDevice.SetValueForKey(
            new NSNumber((int)uiOrientation),
            new NSString("orientation")
        );
        UIApplication.SharedApplication.KeyWindow.RootViewController?.SetNeedsUpdateOfSupportedInterfaceOrientations();
    }

    private static UIInterfaceOrientation ConvertToUiInterfaceOrientation(DisplayOrientation orientation)
    {
        return orientation switch
        {
            DisplayOrientation.Portrait => UIInterfaceOrientation.Portrait,
            DisplayOrientation.Landscape => UIInterfaceOrientation.LandscapeLeft,
            _ => UIInterfaceOrientation.Unknown
        };
    }

    #endif
}