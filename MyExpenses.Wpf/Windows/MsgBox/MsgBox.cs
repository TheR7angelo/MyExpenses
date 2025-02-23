using System.Windows;
using MyExpenses.Wpf.Resources.Resx.Windows.MsgBox;

namespace MyExpenses.Wpf.Windows.MsgBox;

public static class MsgBox
{
    public static MessageBoxResult Show(string caption, string messageBoxText, MessageBoxButton button,
        MsgBoxImage icon, MessageBoxResult defaultResult)
        => ShowCore(caption, messageBoxText, button, icon, defaultResult);

    public static MessageBoxResult Show(string caption, string messageBoxText, MessageBoxButton button,
        MsgBoxImage icon)
        => ShowCore(caption, messageBoxText, button, icon);

    public static MessageBoxResult Show(string caption, string messageBoxText, MessageBoxButton button)
        => ShowCore(caption, messageBoxText, button);

    public static MessageBoxResult Show(string caption, string messageBoxText, MsgBoxImage icon)
        => ShowCore(caption, messageBoxText, icon:icon);

    public static MessageBoxResult Show(string caption, string messageBoxText)
        => ShowCore(caption, messageBoxText);

    public static void Show(string messageBoxText, MsgBoxImage icon)
        => ShowCore(string.Empty, messageBoxText, icon:icon);

    public static MessageBoxResult Show(string messageBoxText, MsgBoxImage icon, MessageBoxButton button)
    {
        return ShowCore(string.Empty, messageBoxText, button, icon);
    }

    public static MessageBoxResult Show(string messageBoxText)
    {
        return ShowCore(string.Empty, messageBoxText);
    }

    private static MessageBoxResult ShowCore(string caption, string messageBoxText,
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
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