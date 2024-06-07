using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    private string DirectoryDatabase { get; } = Path.GetFullPath("Databases");
    private string DatabaseModel { get; }
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; }

    public WelcomePage()
    {
        var directoryDatabaseModel = Path.GetFullPath("Database Models");
        DatabaseModel = Path.Join(directoryDatabaseModel, "Model.sqlite");

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
        const string fileName = "test.sqlite";
        var filePath = Path.Combine(DirectoryDatabase, fileName);
        File.Copy(DatabaseModel, filePath, true);
        ExistingDatabases.Add(new ExistingDatabase { FilePath = filePath });
    }

    //TODO work
    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var removeDatabaseFile = new RemoveDatabaseFile();
        removeDatabaseFile.ExistingDatabases.AddRange(ExistingDatabases);

        removeDatabaseFile.ShowDialog();
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