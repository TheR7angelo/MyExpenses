using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public static class CustomPopupActivityIndicatorHelper
{
    private static CustomPopupActivityIndicator? _customPopupActivityIndicator;
    private static bool _isOpen;

    public static void ShowCustomPopupActivityIndicator(this Page contentPage, string messageToDisplay)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary allocation of a new CustomPopupActivityIndicator instance.
        // This is required to dynamically display a popup activity indicator with a custom message.
        _customPopupActivityIndicator = new CustomPopupActivityIndicator { LabelTextToDisplay = messageToDisplay };
        _customPopupActivityIndicator.Opened += CustomPopupActivityIndicatorOnOpened;
        // contentPage.ShowPopupAsync(_customPopupActivityIndicator);

        _ = contentPage.ShowPopupAsync(_customPopupActivityIndicator);
    }

    private static void CustomPopupActivityIndicatorOnOpened(object? sender, EventArgs e)
    {
        Log.Information("CustomPopupActivityIndicator opened");
        _isOpen = true;
    }

    public static void CloseCustomPopupActivityIndicator()
    {
        while (true)
        {
            if (_isOpen)
            {
                _customPopupActivityIndicator?.CloseAsync();
                _isOpen = false;
                Log.Information("CustomPopupActivityIndicator closed");
                return;
            }
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

    }
}