using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Wpf.Windows.Dialogs.InputDialog;

namespace MyExpenses.Wpf.Windows.Dialogs;

public class DialogService : IDialogService
{
    public bool ShowInputDialog(string title, string defaultText, out MessageBoxInputResult inputResult, out string? input,
        int maxLength = 0, string placeHolder = "")
    {
        var inputDialog = new InputDialogWindow
        {
            Title = title,
            TextBoxText = defaultText,
            EditMode = !string.IsNullOrWhiteSpace(defaultText),
            TextBoxMaxLength = maxLength,
            TextBoxHint = placeHolder
        };

        if (inputDialog.ShowDialog() is true)
        {
            inputResult = inputDialog.MessageBoxInputResult;
            input = inputDialog.TextBoxText;
            return true;
        }

        inputResult = MessageBoxInputResult.None;
        input = null;
        return false;

    }

    public MessageBoxResult ShowMessageBox(string caption, string messageBoxText, MsgBoxImage icon)
    {
        var result = MsgBox.MsgBox.Show(caption, messageBoxText, icon);
        return ReturnResultMessageBox(result);
    }

    public MessageBoxResult ShowMessageBox(string caption, string messageBoxText, MessageBoxButton button, MsgBoxImage icon)
    {
        var b = ConvertButton(button);

        var result = MsgBox.MsgBox.Show(caption, messageBoxText, b, icon);
        return ReturnResultMessageBox(result);
    }

    private static System.Windows.MessageBoxButton ConvertButton(MessageBoxButton button)
    {
        return button switch
        {
            MessageBoxButton.Ok => System.Windows.MessageBoxButton.OK,
            MessageBoxButton.OkCancel => System.Windows.MessageBoxButton.OKCancel,
            MessageBoxButton.YesNoCancel => System.Windows.MessageBoxButton.YesNoCancel,
            MessageBoxButton.YesNo => System.Windows.MessageBoxButton.YesNo,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
        };
    }

    private static MessageBoxResult ReturnResultMessageBox(System.Windows.MessageBoxResult result)
    {
        return result switch
        {
            System.Windows.MessageBoxResult.None => MessageBoxResult.None,
            System.Windows.MessageBoxResult.OK => MessageBoxResult.Ok,
            System.Windows.MessageBoxResult.Cancel => MessageBoxResult.Cancel,
            System.Windows.MessageBoxResult.Yes => MessageBoxResult.Yes,
            System.Windows.MessageBoxResult.No => MessageBoxResult.No,
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }
}