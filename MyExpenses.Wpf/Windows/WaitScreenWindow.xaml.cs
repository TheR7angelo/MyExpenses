using System.Windows;

namespace MyExpenses.Wpf.Windows;

public partial class WaitScreenWindow
{
    public static readonly DependencyProperty WaitMessageProperty = DependencyProperty.Register(nameof(WaitMessage),
        typeof(string), typeof(WaitScreenWindow), new PropertyMetadata(default(string)));

    public WaitScreenWindow()
    {
        InitializeComponent();
    }

    public string WaitMessage
    {
        get => (string)GetValue(WaitMessageProperty);
        set => SetValue(WaitMessageProperty, value);
    }
}