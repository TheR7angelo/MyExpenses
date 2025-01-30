using System.Windows;
using System.Windows.Controls;
using MyExpenses.Wpf.Resources.Resx.Windows.MsgBox;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.MsgBox;

public partial class MsgBoxMessageWindow
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MessageBoxTextProperty =
        DependencyProperty.Register(nameof(MessageBoxText), typeof(string), typeof(MsgBoxMessageWindow),
            new PropertyMetadata(default(string)));

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MessageBoxResultProperty =
        DependencyProperty.Register(nameof(MessageBoxResult), typeof(MessageBoxResult), typeof(MsgBoxMessageWindow),
            new PropertyMetadata(default(MessageBoxResult)));

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty MsgBoxImageProperty = DependencyProperty.Register(nameof(MsgBoxImage),
        typeof(MsgBoxImage), typeof(MsgBoxMessageWindow), new PropertyMetadata(default(MsgBoxImage)));


    public string MessageBoxText
    {
        get => (string)GetValue(MessageBoxTextProperty);
        init => SetValue(MessageBoxTextProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public MsgBoxImage MsgBoxImage
    {
        get => (MsgBoxImage)GetValue(MsgBoxImageProperty);
        set => SetValue(MsgBoxImageProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public MessageBoxResult MessageBoxResult
    {
        get => (MessageBoxResult)GetValue(MessageBoxResultProperty);
        set => SetValue(MessageBoxResultProperty, value);
    }

    public string ButtonCancelContent { get; } = MsgBoxMessageWindowResources.ButtonCancel;
    public string ButtonOkContent { get; } = MsgBoxMessageWindowResources.ButtonOk;
    public string ButtonNoContent { get; } = MsgBoxMessageWindowResources.ButtonNo;
    public string ButtonYesContent { get; } = MsgBoxMessageWindowResources.ButtonYes;

    private readonly Button[] _okOnly;
    private readonly Button[] _okCancel;
    private readonly Button[] _yesNo;
    private readonly Button[] _yesNoCancel;

    public MsgBoxMessageWindow()
    {
        InitializeComponent();

        _okOnly = [ButtonOk];
        _okCancel = [ButtonOk, ButtonCancel];
        _yesNo = [ButtonYes, ButtonNo];
        _yesNoCancel = [ButtonYes, ButtonNo, ButtonCancel];

        SetButtonVisibility();

        this.SetWindowCornerPreference();
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
            MessageBoxButton.OK => (Visible: _okOnly, Collapsed: _yesNoCancel),
            MessageBoxButton.OKCancel => (Visible: _okCancel, Collapsed: _yesNo),
            MessageBoxButton.YesNoCancel => (Visible: _yesNoCancel, Collapsed: _okOnly),
            MessageBoxButton.YesNo => (Visible: _yesNo, Collapsed:_okCancel),
            _ => (Visible: _okOnly,
                Collapsed: _yesNoCancel)
        };

        foreach (var b in buttonSet.Visible) b.Visibility = Visibility.Visible;
        foreach (var b in buttonSet.Collapsed) b.Visibility = Visibility.Collapsed;
    }
}