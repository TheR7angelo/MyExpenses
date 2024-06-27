using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using MyExpenses.Utils.WindowStyle;
using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack),
        typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

    #region MenuItemFile

    public string MenuItemHeaderFile { get; } = MainWindowResources.MenuItemHeaderFile;

    #region MenuItem Database

    public string MenuItemHeaderDatabase { get; } = MainWindowResources.MenuItemHeaderDatabase;

    public string MenuItemHeaderExportDatabase { get; } = MainWindowResources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = MainWindowResources.MenuItemHeaderImportDatabase;

    #endregion

    public string MenuItemHeaderPrevious { get; } = MainWindowResources.MenuItemHeaderPrevious;
    public string MenuItemHeaderSettings { get; } = MainWindowResources.MenuItemHeaderSettings;

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    #endregion

    public MainWindow()
    {
        InitializeComponent();

        var hWnd = new WindowInteropHelper(GetWindow(this)!).EnsureHandle();

        const DwmWindowAttribute attribute = DwmWindowAttribute.DwmwaWindowCornerPreference;

        var osVersion = Environment.OSVersion;
        var preference = osVersion.Version is { Major: >= 10, Build: >= 22000 }
            ? DwmWindowCornerPreference.DwmWcpRound
            : DwmWindowCornerPreference.DwmWcpDefault;

        hWnd.DwmSetWindowAttribute(attribute, ref preference, sizeof(uint));

        Navigator.CanGoBackChanged += Navigator_OnCanGoBackChanged;
    }

    private void Navigator_OnCanGoBackChanged(object? sender, NavigatorEventArgs e)
    {
        CanGoBack = e.CanGoBack;
    }

    private void MenuItemPrevious_OnClick(object sender, RoutedEventArgs e)
        => nameof(FrameBody).GoBack();

    private void FrameBody_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            e.Handled = true;
            nameof(FrameBody).GoBack();
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            e.Handled = true;
            nameof(FrameBody).GoForward();
        }
    }
}