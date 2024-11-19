using CommunityToolkit.Maui.Views;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public static class CustomPopupActivityIndicatorHelper
{
    private static CustomPopupActivityIndicator? _customPopupActivityIndicator;

    public static void ShowCustomPopupActivityIndicator(this ContentPage contentPage)
    {
        _customPopupActivityIndicator = new CustomPopupActivityIndicator();
        contentPage.ShowPopupAsync(_customPopupActivityIndicator);
    }

    public static void CloseCustomPopupActivityIndicator()
    {
        _customPopupActivityIndicator?.CloseAsync();
    }
}