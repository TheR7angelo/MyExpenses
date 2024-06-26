using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.IO;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Resources.Resx.Pages.WelcomePage;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    private string DatabaseModel { get; }
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public WelcomePage()
    {
        var directoryDatabaseModel = Path.GetFullPath("Database Models");
        DatabaseModel = Path.Join(directoryDatabaseModel, "Model.sqlite");

        if (!Directory.Exists(DbContextBackup.LocalDirectoryDatabase)) Directory.CreateDirectory(DbContextBackup.LocalDirectoryDatabase);

        RefreshExistingDatabases();

        InitializeComponent();
    }

    #region Action

    private void ButtonAddDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var addDatabaseFileWindow = new AddDatabaseFileWindow();
        addDatabaseFileWindow.SetExistingDatabase(ExistingDatabases);

        var result = addDatabaseFileWindow.ShowDialog();

        if (result is not true) return;

        var fileName = addDatabaseFileWindow.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DbContextBackup.LocalDirectoryDatabase, fileName);

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

    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoard2Page));
    }

    private async void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var saveLocationWindow = new SaveLocationWindow();
        saveLocationWindow.ShowDialog();

        if (saveLocationWindow.DialogResult is not true) return;

        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;

        try
        {
            //TODO wait screen
            switch (saveLocationWindow.SaveLocationResult)
            {
                case SaveLocation.Local:
                    await SaveToLocal(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    Log.Information("Database was successfully save to local");
                    break;
                case SaveLocation.Dropbox:
                    await SaveToCloudAsync(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    Log.Information("Database was successfully save to cloud");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur please retry");
        }

        //TODO make messagebox result
    }

    private async void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var saveLocationWindow = new SaveLocationWindow();
        saveLocationWindow.ShowDialog();

        if (saveLocationWindow.DialogResult is not true) return;

        try
        {
            //TODO wait screen
            switch (saveLocationWindow.SaveLocationResult)
            {
                case SaveLocation.Local:
                    await ImportFromLocal();
                    Log.Information("Local database was successfully imported");
                    break;
                case SaveLocation.Dropbox:
                    await ImportFromCloudAsync();
                    Log.Information("Cloud Database was successfully imported");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExistingDatabases();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur please retry");
        }

        //TODO make messagebox result
    }

    private async Task ImportFromCloudAsync()
    {
        var dropboxService = new DropboxService();
        var metadatas = await dropboxService.ListFile(DbContextBackup.CloudDirectoryBackupDatabase);
        metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DbContextBackup.Extension));

        var existingDatabase = metadatas.Select(s => new ExistingDatabase { FilePath = s.PathDisplay });
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabase);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;

        //TODO work, ask user to confirm copy if file already exist
        var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FilePath!);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

            var temp = await dropboxService.DownloadFileAsync(file);
            File.Copy(temp, newFilePath, true);
        }
    }

    private async Task ImportFromLocal()
    {
        var dialog = new SqliteFileDialog(multiSelect:true);
        var files = dialog.GetFiles();

        if (files is null || files.Length.Equals(0)) return;

        //TODO work, ask user to confirm copy if file already exist
        await Parallel.ForEachAsync(files, (file, _) =>
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

            File.Copy(file, newFilePath, true);

            return default;
        });
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

    private static async Task SaveToCloudAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await ExportToCloudFileAsync(existingDatabasesSelected.First());
        else await ExportToCloudDirectoryAsync(existingDatabasesSelected);
    }

    private static async Task ExportToCloudDirectoryAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        var dropboxService = new DropboxService();
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            await dropboxService.UploadFileAsync(existingDatabase.FilePath!, DbContextBackup.CloudDirectoryBackupDatabase);
        }
    }

    private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var dropboxService = new DropboxService();
        await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath!, DbContextBackup.CloudDirectoryBackupDatabase);
    }

    private static async Task SaveToLocal(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await ExportToLocalFileAsync(existingDatabasesSelected.First());
        else await ExportToLocalDirectoryAsync(existingDatabasesSelected);
    }

    private static async Task ExportToLocalDirectoryAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        var folderDialog = new FolderDialog();
        var selectedFolder = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedFolder)) return;

        foreach (var existingDatabase in existingDatabasesSelected)
        {
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            await Task.Run(() => File.Copy(existingDatabase.FilePath!, newFilePath, true));
        }
    }

    private static async Task ExportToLocalFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var sqliteDialog = new SqliteFileDialog();
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog)) return;

        selectedDialog = Path.ChangeExtension(selectedDialog, DbContextBackup.Extension);
        var selectedFilePath = existingDatabasesSelected.FilePath!;
        await Task.Run(() => File.Copy(selectedFilePath, selectedDialog, true));
    }

    private void RefreshExistingDatabases()
    {
        ExistingDatabases.Clear();
        var existingDatabases = DbContextBackup.GetExistingDatabase()
                .OrderByDescending(s => s.FileNameWithoutExtension);

        ExistingDatabases.AddRange(existingDatabases);
    }

    #endregion
}