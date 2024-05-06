using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Windows.MsgBox;

public partial class MsgBoxMessageWindow
{
    public static readonly DependencyProperty MessageBoxTextProperty =
        DependencyProperty.Register(nameof(MessageBoxText), typeof(string), typeof(MsgBoxMessageWindow),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty MessageBoxResultProperty =
        DependencyProperty.Register(nameof(MessageBoxResult), typeof(MessageBoxResult), typeof(MsgBoxMessageWindow),
            new PropertyMetadata(default(MessageBoxResult)));

    public static readonly DependencyProperty MsgBoxImageProperty = DependencyProperty.Register(nameof(MsgBoxImage),
        typeof(MsgBoxImage), typeof(MsgBoxMessageWindow), new PropertyMetadata(default(MsgBoxImage)));


    public string MessageBoxText
    {
        get => (string)GetValue(MessageBoxTextProperty);
        set => SetValue(MessageBoxTextProperty, value);
    }

    public MsgBoxImage MsgBoxImage
    {
        get => (MsgBoxImage)GetValue(MsgBoxImageProperty);
        set => SetValue(MsgBoxImageProperty, value);
    }

    public MessageBoxResult MessageBoxResult
    {
        get => (MessageBoxResult)GetValue(MessageBoxResultProperty);
        set => SetValue(MessageBoxResultProperty, value);
    }

    public MsgBoxMessageWindow()
    {
        InitializeComponent();
        SetButtonVisibility();
    }

    private void ButtonYes_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageBoxResult.Yes);

    private void ButtonNo_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageBoxResult.No);

    private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageBoxResult.OK);

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => SetResult(MessageBoxResult.Cancel);

    private void SetResult(MessageBoxResult result)
    {
        MessageBoxResult = result;
        Close();
    }

    internal void SetButtonVisibility(MessageBoxButton button = MessageBoxButton.OK)
    {
        var buttonToCollapse = button switch
        {
            MessageBoxButton.OK => [ButtonYes, ButtonNo, ButtonCancel],
            MessageBoxButton.OKCancel => [ButtonYes, ButtonNo],
            MessageBoxButton.YesNoCancel => [ButtonOk],
            MessageBoxButton.YesNo => [ButtonOk, ButtonCancel],
            _ => new List<Button> { ButtonYes, ButtonNo, ButtonOk, ButtonCancel }
        };
        var buttonToVisible = button switch
        {
            MessageBoxButton.OK => [ButtonOk],
            MessageBoxButton.OKCancel => [ButtonOk, ButtonCancel],
            MessageBoxButton.YesNoCancel => [ButtonYes, ButtonNo, ButtonCancel],
            MessageBoxButton.YesNo => [ButtonYes, ButtonNo],
            _ => new List<Button>()
        };

        foreach (var b in buttonToCollapse) b.Visibility = Visibility.Collapsed;
        foreach (var b in buttonToVisible) b.Visibility = Visibility.Visible;
    }
}