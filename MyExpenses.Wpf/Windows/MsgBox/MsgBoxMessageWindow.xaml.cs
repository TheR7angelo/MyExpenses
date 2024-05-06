using System.Windows;

namespace MyExpenses.Wpf.Windows.MsgBox;

public partial class MsgBoxMessageWindow
{
    public static readonly DependencyProperty MsgBoxImageProperty = DependencyProperty.Register(nameof(MsgBoxImage),
        typeof(MsgBoxImage), typeof(MsgBoxMessageWindow), new PropertyMetadata(default(MsgBoxImage)));

    public static readonly DependencyProperty MessageBoxTextProperty =
        DependencyProperty.Register(nameof(MessageBoxText), typeof(string), typeof(MsgBoxMessageWindow),
            new PropertyMetadata(default(string)));

    public MessageBoxResult MessageBoxResult { get; set; }

    public MsgBoxImage MsgBoxImage
    {
        get => (MsgBoxImage)GetValue(MsgBoxImageProperty);
        set => SetValue(MsgBoxImageProperty, value);
    }

    public string MessageBoxText
    {
        get => (string)GetValue(MessageBoxTextProperty);
        set => SetValue(MessageBoxTextProperty, value);
    }

    public MsgBoxMessageWindow()
    {
        InitializeComponent();
    }
}