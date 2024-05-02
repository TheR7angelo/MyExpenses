using MyExpenses.Wpf.Resources.Resx.Windows.MainWindow;

namespace MyExpenses.Wpf;

public partial class MainWindow
{
    #region MenuItemFile

    public string MenuItemHeaderFile { get; } = MainWindowResources.MenuItemHeaderFile;

    #region MenuItem Database

    public string MenuItemHeaderDatabase { get; } = MainWindowResources.MenuItemHeaderDatabase;

    public string MenuItemHeaderExportDatabase { get; } = MainWindowResources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = MainWindowResources.MenuItemHeaderImportDatabase;

    #endregion

    #endregion

    public MainWindow()
    {
        InitializeComponent();
    }
}