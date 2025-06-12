using CommunityToolkit.Maui.Extensions;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public static class CustomPopupActivityIndicatorHelper
{
    // private static CustomPopupActivityIndicator? _customPopupActivityIndicator;
    // private static bool _isOpen;

    public static void ShowCustomPopupActivityIndicator(this Page contentPage, string messageToDisplay)
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // Necessary allocation of a new CustomPopupActivityIndicator instance.
        // // This is required to dynamically display a popup activity indicator with a custom message.
        // _customPopupActivityIndicator = new CustomPopupActivityIndicator { LabelTextToDisplay = messageToDisplay };
        // _customPopupActivityIndicator.Opened += CustomPopupActivityIndicatorOnOpened;
        // _customPopupActivityIndicator.Closed += CustomPopupActivityIndicatorOnClosed;
        // contentPage.ShowPopup(_customPopupActivityIndicator);
    }

    // private static void CustomPopupActivityIndicatorOnClosed(object? sender, EventArgs e)
    // {
    //     Log.Information("CustomPopupActivityIndicator closed");
    //     _isOpen = false;
    // }
    //
    // private static void CustomPopupActivityIndicatorOnOpened(object? sender, EventArgs e)
    // {
    //     Log.Information("CustomPopupActivityIndicator opened");
    //     _isOpen = true;
    // }

    public static void CloseCustomPopupActivityIndicator()
    {
        // Log.Information("CustomPopupActivityIndicator close requested");
        // _customPopupActivityIndicator?.CloseAsync();
    }
}