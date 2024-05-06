using System.Windows;
using System.Windows.Controls;
using MyExpenses.Wpf.Resources.Resx.Windows.MsgBox;

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

    public string ButtonCancelContent { get; } = MsgBoxMessageWindowResources.ButtonCancel;
    public string ButtonOkContent { get; } = MsgBoxMessageWindowResources.ButtonOk;
    public string ButtonNoContent { get; } = MsgBoxMessageWindowResources.ButtonNo;
    public string ButtonYesContent { get; } = MsgBoxMessageWindowResources.ButtonYes;

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
        var buttonSet = button switch
        {
            MessageBoxButton.OK => (Visible: [ButtonOk], Collapsed: [ButtonYes, ButtonNo, ButtonCancel]),
            MessageBoxButton.OKCancel => (Visible: [ButtonOk, ButtonCancel], Collapsed: [ButtonYes, ButtonNo]),
            MessageBoxButton.YesNoCancel => (Visible: [ButtonYes, ButtonNo, ButtonCancel], Collapsed: [ButtonOk]),
            MessageBoxButton.YesNo => (Visible: [ButtonYes, ButtonNo], Collapsed: [ButtonOk, ButtonCancel]),
            _ => (Visible: new List<Button> { ButtonOk },
                Collapsed: new List<Button> { ButtonYes, ButtonNo, ButtonCancel })
        };

        foreach (var b in buttonSet.Visible) b.Visibility = Visibility.Visible;
        foreach (var b in buttonSet.Collapsed) b.Visibility = Visibility.Collapsed;
    }
}