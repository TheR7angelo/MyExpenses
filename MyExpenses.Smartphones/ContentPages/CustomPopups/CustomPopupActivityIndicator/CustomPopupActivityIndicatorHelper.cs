using UraniumUI.Dialogs.Mopups;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;

public static class CustomPopupActivityIndicatorHelper
{
    public static async Task ShowCustomPopupActivityIndicatorAsync(this Page contentPage, string title, string messageToDisplay, Func<Task> function)
    {
        using (await contentPage.DisplayProgressAsync(title, messageToDisplay))
        {
            await function.Invoke();
        }
    }
}