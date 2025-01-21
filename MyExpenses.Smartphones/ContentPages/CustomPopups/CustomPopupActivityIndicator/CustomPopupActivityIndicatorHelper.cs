using CommunityToolkit.Maui.Views;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public static class CustomPopupActivityIndicatorHelper
{
    private static CustomPopupActivityIndicator? _customPopupActivityIndicator;

    public static void ShowCustomPopupActivityIndicator(this ContentPage contentPage, string messageToDisplay)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary allocation of a new CustomPopupActivityIndicator instance.
        // This is required to dynamically display a popup activity indicator with a custom message.
        _customPopupActivityIndicator = new CustomPopupActivityIndicator { LabelTextToDisplay = messageToDisplay };
        contentPage.ShowPopupAsync(_customPopupActivityIndicator);
    }

    public static void CloseCustomPopupActivityIndicator()
    {
        _customPopupActivityIndicator?.CloseAsync();
    }
}