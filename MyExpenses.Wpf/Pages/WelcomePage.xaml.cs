using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.Pages.WelcomePage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    private string DirectoryDatabase { get; } = Path.GetFullPath("Databases");
    private string DatabaseModel { get; }
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public WelcomePage()
    {
        var directoryDatabaseModel = Path.GetFullPath("Database Models");
        DatabaseModel = Path.Join(directoryDatabaseModel, "Model.sqlite");

        if (!Directory.Exists(DirectoryDatabase)) Directory.CreateDirectory(DirectoryDatabase);

        RefreshExistingDatabases();

        InitializeComponent();
    }

    #region Action

    //TODO work
    private void ButtonAddDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var addDatabaseFileWindow = new AddDatabaseFileWindow();
        addDatabaseFileWindow.SetExistingDatabase(ExistingDatabases);

        var result = addDatabaseFileWindow.ShowDialog();

        if (result is not true) return;

        var fileName = addDatabaseFileWindow.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DirectoryDatabase, fileName);
        File.Copy(DatabaseModel, filePath, true);

        ExistingDatabases.AddAndSort(new ExistingDatabase { FilePath = filePath }, s => s.FileNameWithoutExtension!);
    }

    //TODO make save automatically
    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoardPage));
    }

    //TODO work
    private void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    //TODO work
    private void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    //TODO work
    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var removeDatabaseFile = new RemoveDatabaseFile();
        removeDatabaseFile.ExistingDatabases.AddRange(ExistingDatabases);

        removeDatabaseFile.ShowDialog();

        if (removeDatabaseFile.DialogResult is not true) return;

        var response = MsgBox.Show(WelcomePageResources.DeleteDatabaseQuestion, MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        foreach (var existingDatabase in removeDatabaseFile.ExistingDatabasesToDelete)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            File.Delete(existingDatabase.FilePath);
        }

        RefreshExistingDatabases();
    }

    #endregion

    #region Function

    private void RefreshExistingDatabases()
    {
        ExistingDatabases.Clear();
        var existingDatabases = Directory
            .GetFiles(DirectoryDatabase, "*.sqlite")
            .OrderByDescending(s => s)
            .Select(s => new ExistingDatabase { FilePath = s } );
        ExistingDatabases.AddRange(existingDatabases);
    }

    #endregion
}