using System.Windows;
using MyExpenses.Wpf.Resources.Resx.Windows.MsgBox;

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

    public static void Show(string messageBoxText, MsgBoxImage icon)
        => ShowCore(messageBoxText, string.Empty, icon:icon);

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
        caption = string.IsNullOrWhiteSpace(caption)
            ? icon switch
            {
                MsgBoxImage.Error => MsgBoxMessageWindowResources.CaptionError,
                MsgBoxImage.Question => MsgBoxMessageWindowResources.CaptionQuestion,
                MsgBoxImage.Warning => MsgBoxMessageWindowResources.CaptionWarning,
                MsgBoxImage.Information => MsgBoxMessageWindowResources.CaptionInformation,
                MsgBoxImage.Check => MsgBoxMessageWindowResources.CaptionCheck,
                MsgBoxImage.Asterisk => string.Empty,
                MsgBoxImage.Exclamation => string.Empty,
                MsgBoxImage.Stop => string.Empty,
                MsgBoxImage.Hand => string.Empty,
                MsgBoxImage.None => string.Empty,
                _ => string.Empty
            }
            : caption;

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