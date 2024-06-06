using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    private string DirectoryDatabase { get; } = Path.GetFullPath("Databases");
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; }

    public WelcomePage()
    {
        if (!Directory.Exists(DirectoryDatabase)) Directory.CreateDirectory(DirectoryDatabase);

        var existingDatabases = Directory
            .GetFiles(DirectoryDatabase, "*.sqlite")
            .Select(s => new ExistingDatabase { FilePath = s } );

        ExistingDatabases = [..existingDatabases];

        InitializeComponent();
    }

    //TODO work
    private void ButtonAddDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    //TODO make save automatically
    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoardPage));
    }
}