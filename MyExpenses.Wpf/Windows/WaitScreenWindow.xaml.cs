using System.Windows;
using System.Windows.Interop;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.WaitScreenWindow;

namespace MyExpenses.Wpf.Windows;

public partial class WaitScreenWindow
{
    public static readonly DependencyProperty WaitMessageProperty = DependencyProperty.Register(nameof(WaitMessage),
        typeof(string), typeof(WaitScreenWindow), new PropertyMetadata(default(string)));

    public string WaitScreenWindowTitle { get; } = WaitScreenWindowResources.WaitScreenWindowTitle;

    public WaitScreenWindow()
    {
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();
        hWnd.SetWindowCornerPreference(DwmWindowCornerPreference.Round);
    }

    public string WaitMessage
    {
        get => (string)GetValue(WaitMessageProperty);
        set => SetValue(WaitMessageProperty, value);
    }
}