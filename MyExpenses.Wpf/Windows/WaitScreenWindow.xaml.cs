using System.Windows;
using MyExpenses.Wpf.Resources.Resx.Windows.WaitScreenWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class WaitScreenWindow
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty WaitMessageProperty = DependencyProperty.Register(nameof(WaitMessage),
        typeof(string), typeof(WaitScreenWindow), new PropertyMetadata(default(string)));

    public string WaitScreenWindowTitle { get; } = WaitScreenWindowResources.WaitScreenWindowTitle;

    public WaitScreenWindow()
    {
        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    public string WaitMessage
    {
        get => (string)GetValue(WaitMessageProperty);
        set => SetValue(WaitMessageProperty, value);
    }
}