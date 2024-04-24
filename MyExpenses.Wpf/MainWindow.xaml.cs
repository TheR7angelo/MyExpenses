namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public string MenuItemHeaderFile { get; } = Ressources.Resx.MainWindow.MainWindowRessources.MenuItemHeaderFile;
    public string MenuItemHeaderExportDatabase { get; } = Ressources.Resx.MainWindow.MainWindowRessources.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = Ressources.Resx.MainWindow.MainWindowRessources.MenuItemHeaderImportDatabase;

    public MainWindow()
    {
        InitializeComponent();
    }
}