namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public string MenuItemHeaderFile { get; } = MyExpenses.Wpf.Resources.Resx.MainWindow.MainWindowResources.MenuItemHeaderFile;
    public string MenuItemHeaderExportDatabase { get; } = MyExpenses.Wpf.Resources.Resx.MainWindow.MainWindowResources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = MyExpenses.Wpf.Resources.Resx.MainWindow.MainWindowResources.MenuItemHeaderImportDatabase;

    public MainWindow()
    {
        InitializeComponent();
    }
}