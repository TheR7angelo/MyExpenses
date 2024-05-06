using System.Windows;

namespace MyExpenses.Wpf.Windows.MsgBox;

public partial class MsgBoxMessageWindow
{
    public static readonly DependencyProperty MsgBoxImageProperty = DependencyProperty.Register(nameof(MsgBoxImage),
        typeof(MsgBoxImage), typeof(MsgBoxMessageWindow), new PropertyMetadata(default(MsgBoxImage)));

    public MessageBoxResult MessageBoxResult { get; set; }

    public MsgBoxImage MsgBoxImage
    {
        get => (MsgBoxImage)GetValue(MsgBoxImageProperty);
        set => SetValue(MsgBoxImageProperty, value);
    }

    public MsgBoxMessageWindow()
    {
        InitializeComponent();
    }
}