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

        //TODO message
        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocationWindow.SaveLocationResult)
            {
                case SaveLocation.Local:
                    waitScreenWindow.WaitMessage = "Saving to local storage... Please wait";
                    waitScreenWindow.Show();
                    await SaveToLocal(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = "Uploading to Dropbox... Please wait";
                    waitScreenWindow.Show();
                    await SaveToCloudAsync(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            waitScreenWindow.Close();

            MsgBox.Show("Database backup operation was successful", MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show("An error occurred. Please try again", MsgBoxImage.Warning);
        }
    }

    private async void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        var saveLocationWindow = new SaveLocationWindow();
        saveLocationWindow.ShowDialog();

        if (saveLocationWindow.DialogResult is not true) return;

        //TODO message
        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocationWindow.SaveLocationResult)
            {
                case SaveLocation.Local:
                    waitScreenWindow.WaitMessage = "Import from local storage... Please wait";
                    waitScreenWindow.Show();
                    await ImportFromLocal();
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = "Import from cloud storage... Please wait";
                    waitScreenWindow.Show();
                    await ImportFromCloudAsync();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExistingDatabases();

            waitScreenWindow.Close();
            MsgBox.Show("Database import operation was successful", MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show("An error occurred. Please try again", MsgBoxImage.Warning);
        }
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
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
            await dropboxService.UploadFileAsync(existingDatabase.FilePath!, DbContextBackup.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
        }
    }

    private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var dropboxService = new DropboxService();
        Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
        await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath!, DbContextBackup.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
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

        if (string.IsNullOrEmpty(selectedFolder))
        {
            Log.Warning("Export cancelled. No directory selected");
            return;
        }

        foreach (var existingDatabase in existingDatabasesSelected)
        {
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
            await Task.Run(() => File.Copy(existingDatabase.FilePath!, newFilePath, true));
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
        }
    }

    private static async Task ExportToLocalFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var sqliteDialog = new SqliteFileDialog(defaultFileName: existingDatabasesSelected.FileName);
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        selectedDialog = Path.ChangeExtension(selectedDialog, DbContextBackup.Extension);
        var selectedFilePath = existingDatabasesSelected.FilePath!;
        Log.Information("Starting to copy database to {SelectedDialog}", selectedDialog);
        await Task.Run(() =>
        {
            File.Copy(selectedFilePath, selectedDialog, true);
        });
        Log.Information("Database successfully copied to local storage");
    }

private static async Task ImportFromCloudAsync()
{
    Log.Information("Starting to import the database from cloud storage");
    var dropboxService = new DropboxService();
    var metadatas = await dropboxService.ListFile(DbContextBackup.CloudDirectoryBackupDatabase);
    metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DbContextBackup.Extension));

    var existingDatabase = metadatas.Select(s => new ExistingDatabase { FilePath = s.PathDisplay });
    var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
    selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabase);
    selectDatabaseFileWindow.ShowDialog();

    if (selectDatabaseFileWindow.DialogResult is not true)
    {
        Log.Warning("Import cancelled. No database selected");
        return;
    }

    var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FilePath!);
    foreach (var file in files)
    {
        var fileName = Path.GetFileName(file);
        var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

        var temp = await dropboxService.DownloadFileAsync(file);
        Log.Information("Downloading {FileName} from cloud storage", fileName);
        File.Copy(temp, newFilePath, true);
        Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
    }
}

private static async Task ImportFromLocal()
{
    Log.Information("Starting to import the database from local storage");
    var dialog = new SqliteFileDialog(multiSelect:true);
    var files = dialog.GetFiles();

    if (files is null || files.Length.Equals(0))
    {
        Log.Warning("Import cancelled. No files selected");
        return;
    }

    await Parallel.ForEachAsync(files, (file, _) =>
    {
        var fileName = Path.GetFileName(file);
        var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

        Log.Information("Copying {FileName} to local storage", fileName);
        File.Copy(file, newFilePath, true);
        Log.Information("Successfully copied {FileName} to local storage", fileName);

        return default;
    });
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