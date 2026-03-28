using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Wpf.Windows.Dialogs.InputDialog;

namespace MyExpenses.Wpf.Windows.Dialogs;

public class DialogService : IDialogService
{
    public bool ShowInputDialog(string title, string defaultText, out MessageBoxResult result, out string? input,
        int maxLength = 0)
    {
        var inputDialog = new InputDialogWindow
        {
            Title = title,
            TextBoxText = defaultText,
            TextBoxMaxLength = maxLength
        };

        if (inputDialog.ShowDialog() is true)
        {
            result = inputDialog.MessageBoxResult;
            input = inputDialog.TextBoxText;
            return true;
        }

        result = MessageBoxResult.None;
        input = null;
        return false;

    }
}