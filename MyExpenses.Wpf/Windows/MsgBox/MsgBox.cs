using System.Windows;

namespace MyExpenses.Wpf.Windows.MsgBox;

public static class MsgBox
{
    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
        MsgBoxImage icon, MessageBoxResult defaultResult)
        => ShowCore(messageBoxText, caption, button, icon, defaultResult);

    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button,
        MsgBoxImage icon)
        => ShowCore(messageBoxText, caption, button, icon);

    public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        => ShowCore(messageBoxText, caption, button);

    public static MessageBoxResult Show(string messageBoxText, string caption, MsgBoxImage icon)
        => ShowCore(messageBoxText, caption, icon:icon);

    public static MessageBoxResult Show(string messageBoxText, string caption)
        => ShowCore(messageBoxText, caption);

    public static MessageBoxResult Show(string messageBoxText, MsgBoxImage icon)
    {
        return ShowCore(messageBoxText, string.Empty, icon:icon);
    }

    public static MessageBoxResult Show(string messageBoxText, MsgBoxImage icon, MessageBoxButton button)
    {
        return ShowCore(messageBoxText, string.Empty, icon:icon, button:button);
    }

    public static MessageBoxResult Show(string messageBoxText)
    {
        return ShowCore(messageBoxText, string.Empty);
    }

    private static MessageBoxResult ShowCore(string messageBoxText, string caption,
        MessageBoxButton button = MessageBoxButton.OK, MsgBoxImage icon = MsgBoxImage.None,
        MessageBoxResult defaultResult = MessageBoxResult.None)
    {
        var msgBoxMessageWindow = new MsgBoxMessageWindow
        {
            MessageBoxText = messageBoxText,
            Title = caption,
            MsgBoxImage = icon,
            MessageBoxResult = defaultResult
        };

        msgBoxMessageWindow.SetButtonVisibility(button);
        msgBoxMessageWindow.ShowDialog();

        var result = msgBoxMessageWindow.MessageBoxResult;
        return result;
    }
}