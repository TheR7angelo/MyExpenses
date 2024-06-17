using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.Pages.WelcomePage;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    private string DirectoryDatabase { get; } = DbContextBackup.DirectoryDatabase;
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

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DatabaseModel, filePath, true);

            using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            context.SaveChanges();

            ExistingDatabases.AddAndSort(new ExistingDatabase { FilePath = filePath },
                s => s.FileNameWithoutExtension!);

            MsgBox.Show(WelcomePageResources.MessageBoxCreateNewDatabaseSuccess, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur");
            MsgBox.Show(WelcomePageResources.MessageBoxCreateNewDatabaseError, MsgBoxImage.Error);
        }
    }

    //TODO make save automatically
    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoard2Page));
    }

    //TODO work
    private void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;

        var folderDialog = new FolderDialog();
        var selectedFolder = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedFolder)) return;

        foreach (var existingDatabase in selectDatabaseFileWindow.ExistingDatabasesSelected)
        {
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            File.Copy(existingDatabase.FilePath!, newFilePath, true);
        }

        //TODO make messagebox result
    }

    private void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
        var dialog = new SqliteFileDialog();
        var file = dialog.GetFile();

        if (string.IsNullOrEmpty(file)) return;

        var fileName = Path.GetFileName(file);
        var newFilePath = Path.Join(DirectoryDatabase, fileName);

        //TODO work, ask user to confirm copy if file already exist
        File.Copy(file, newFilePath, true);
    }

    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;

        var response = MsgBox.Show(WelcomePageResources.DeleteDatabaseQuestion, MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        foreach (var existingDatabase in selectDatabaseFileWindow.ExistingDatabasesSelected)
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
        var existingDatabases = DbContextBackup.GetExistingDatabase()
                .OrderByDescending(s => s.FileNameWithoutExtension);

        ExistingDatabases.AddRange(existingDatabases);
    }

    #endregion
}