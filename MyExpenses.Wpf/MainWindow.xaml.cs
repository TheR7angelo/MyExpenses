namespace MyExpenses.Wpf;

public partial class MainWindow
{
    public string MenuItemHeaderFile { get; } = Ressources.Resx.MainWindow.MainWindowRessource.MenuItemHeaderFile;
    public string MenuItemHeaderExportDatabase { get; } = Ressources.Resx.MainWindow.MainWindowRessource.MenuItemHeaderExportDatabase;
    public string MenuItemHeaderImportDatabase { get; } = Ressources.Resx.MainWindow.MainWindowRessource.MenuItemHeaderImportDatabase;

    public MainWindow()
    {
        InitializeComponent();
    }
}