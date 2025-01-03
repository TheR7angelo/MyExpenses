using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Resources.Resx.Pages.WelcomePage;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.AutoUpdaterGitHub;
using MyExpenses.Wpf.Windows.MsgBox;
using MyExpenses.Wpf.Windows.SaveLocationWindow;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class WelcomePage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public WelcomePage()
    {
        RefreshExistingDatabases();

        InitializeComponent();

        AutoUpdaterGitHub.CheckUpdateGitHub();

        MainWindow.VaccumDatabase += MainWindow_OnVaccumDatabase;
    }

    private void MainWindow_OnVaccumDatabase()
        => _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync();

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
            File.Copy(DbContextBackup.LocalFilePathDataBaseModel, filePath, true);

            using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            context.SaveChanges();

            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            Log.Information("New database was successfully added");
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

        Log.Information("Connection to the database : \"{FileName}\"", existingDatabase.FileNameWithoutExtension);

        DataBaseContext.FilePath = existingDatabase.FilePath;
        // nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoard2Page));
        nameof(MainWindow.FrameBody).NavigateTo(typeof(DashBoardPage));
    }

    private void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonExportDataBase();

    private void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonImportDataBase();

    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonRemoveDataBase();

    #endregion

    #region Function

    private static bool ConfirmDeletion(string message)
    {
        var response = MsgBox.Show(message, MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        return response == MessageBoxResult.Yes;
    }

    private static async Task DeleteCloudFilesAsync(List<ExistingDatabase> databases)
    {
        var files = databases.Select(db => db.FileName).ToArray();
        Log.Information("Preparing to delete the following files: {Files}", files);

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        _ = await dropboxService.DeleteFilesAsync(files, DbContextBackup.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from cloud");
    }

    private static void DeleteLocalDatabases(List<ExistingDatabase> databases)
    {
        foreach (var database in databases.Where(database =>
                     !string.IsNullOrEmpty(database.FilePath) && File.Exists(database.FilePath)))
        {
            File.Delete(database.FilePath);

            var backupDirectory = Path.Join(DbContextBackup.LocalDirectoryBackupDatabase,
                database.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory))
            {
                Directory.Delete(backupDirectory, true);
            }
        }
    }

    private static async Task ExportToCloudDirectoryAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage",
                existingDatabase.FileName);
            _ = await dropboxService.UploadFileAsync(existingDatabase.FilePath,
                DbContextBackup.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage",
                existingDatabase.FileName);
        }
    }

    private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
        _ = await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath,
            DbContextBackup.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
    }

    private static async Task ExportToLocalFolderAsync(List<ExistingDatabase> existingDatabasesSelected,
        bool isCompress)
    {
        var folderDialog = new FolderDialog();
        var selectedDialog = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        Log.Information("Starting to export database to {SelectedDialog}", selectedDialog);

        var failedExistingDatabases = new List<ExistingDatabase>();
        await Task.Run(async () =>
        {
            foreach (var existingDatabase in existingDatabasesSelected)
            {
                Log.Information("Starting to export {ExistingDatabaseFileName}",
                    existingDatabase.FileNameWithoutExtension);
                var success = await existingDatabase.ToFolderAsync(selectedDialog, isCompress);
                if (!success) failedExistingDatabases.Add(existingDatabase);
                else
                    Log.Information("Successfully exported {ExistingDatabaseFileName}",
                        existingDatabase.FileNameWithoutExtension);
            }
        });

        if (failedExistingDatabases.Count > 0)
        {
            Log.Information("Failed to export some database to local folder");
            MsgBox.Show(WelcomePageResources.MessageBoxErrorExportToLocalFolder, MsgBoxImage.Error,
                MessageBoxButton.OK);
            return;
        }

        Log.Information("Database successfully copied to local storage");

        var response = MsgBox.Show(WelcomePageResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedDialog.StartFile();
    }

    private List<ExistingDatabase>? GetSelectedDatabases()
    {
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);
        selectDatabaseFileWindow.ShowDialog();

        return selectDatabaseFileWindow.DialogResult == true
            ? selectDatabaseFileWindow.ExistingDatabasesSelected
            : null;
    }

    private async Task HandleButtonExportDataBase()
    {
        var saveLocation = SaveLocationUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;
        if (selectDatabaseFileWindow.ExistingDatabasesSelected.Count.Equals(0)) return;

        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonExportDataBaseWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await SaveToLocalDatabase(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Folder:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonExportDataBaseWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(selectDatabaseFileWindow.ExistingDatabasesSelected, false);
                    break;

                case SaveLocation.Compress:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonExportDataBaseWaitMessageExportToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(selectDatabaseFileWindow.ExistingDatabasesSelected, true);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonExportDataBaseWaitMessageExportToCloud;
                    waitScreenWindow.Show();
                    await SaveToCloudAsync(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                case null:
                case SaveLocation.Local:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            waitScreenWindow.Close();

            MsgBox.Show(WelcomePageResources.ButtonExportDataBaseSuccess, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(WelcomePageResources.ButtonExportDataBaseError, MsgBoxImage.Warning);
        }
    }

    private async Task HandleButtonImportDataBase()
    {
        var saveLocation = SaveLocationUtils.GetImportSaveLocation(SaveLocationMode.LocalDropbox);
        if (saveLocation is null) return;

        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonImportDataBaseWaitMessageImportFromLocal;
                    waitScreenWindow.Show();
                    await ImportFromLocalAsync();
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonImportDataBaseWaitMessageImportFromCloud;
                    waitScreenWindow.Show();
                    await ImportFromCloudAsync();
                    break;

                case null:
                case SaveLocation.Folder:
                case SaveLocation.Database:
                case SaveLocation.Compress:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExistingDatabases();

            waitScreenWindow.Close();
            MsgBox.Show(WelcomePageResources.ButtonImportDataBaseImportSucess, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(WelcomePageResources.ButtonImportDataBaseError, MsgBoxImage.Warning);
        }
    }

    private async Task HandleButtonRemoveDataBase()
    {
        var selectedDatabases = GetSelectedDatabases();
        if (selectedDatabases is null || selectedDatabases.Count is 0) return;

        var confirmLocalDeletion = ConfirmDeletion(WelcomePageResources.DeleteDatabaseQuestion);
        if (!confirmLocalDeletion) return;

        DeleteLocalDatabases(selectedDatabases);
        RefreshExistingDatabases();

        var confirmCloudDeletion = ConfirmDeletion(WelcomePageResources.MessageBoxDeleteCloudQuestion);
        if (!confirmCloudDeletion) return;

        await DeleteCloudFilesAsync(selectedDatabases);

        ShowSuccessMessage(WelcomePageResources.MessageBoxDeleteCloudQuestionSuccess);
    }

    private static async Task SaveToLocalDatabase(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1)
            await ExportToLocalDatabaseFileAsync(existingDatabasesSelected.First());
        else await ExportToLocalDirectoryDatabaseAsync(existingDatabasesSelected);
    }

    private static async Task ExportToLocalDirectoryDatabaseAsync(List<ExistingDatabase> existingDatabasesSelected)
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
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName,
                newFilePath);
            await Task.Run(() => File.Copy(existingDatabase.FilePath, newFilePath, true));
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}",
                existingDatabase.FileName, newFilePath);
        }

        var response = MsgBox.Show(WelcomePageResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedFolder.StartFile();
    }

    private static async Task ExportToLocalDatabaseFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var sqliteDialog = new SqliteFileDialog(defaultFileName: existingDatabasesSelected.FileName);
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        selectedDialog = Path.ChangeExtension(selectedDialog, DbContextBackup.Extension);
        var selectedFilePath = existingDatabasesSelected.FilePath;
        Log.Information("Starting to copy database to {SelectedDialog}", selectedDialog);
        await Task.Run(() => { File.Copy(selectedFilePath, selectedDialog, true); });
        Log.Information("Database successfully copied to local storage");

        var parentDirectory = Path.GetDirectoryName(selectedDialog)!;
        var response = MsgBox.Show(WelcomePageResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) parentDirectory.StartFile();
    }

    private static async Task ImportFromCloudAsync()
    {
        Log.Information("Starting to import the database from cloud storage");
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        var metadatas = await dropboxService.ListFileAsync(DbContextBackup.CloudDirectoryBackupDatabase);
        metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DbContextBackup.Extension));

        var existingDatabases = metadatas.Select(s => new ExistingDatabase(s.PathDisplay)).ToList();
        foreach (var existingDatabase in existingDatabases)
        {
            var filePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, existingDatabase.FileName);
            var localDatabase = new ExistingDatabase(filePath);
            existingDatabase.SyncStatus = await localDatabase.CheckStatus(ProjectSystem.Wpf);
        }

        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabases);
        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true)
        {
            Log.Warning("Import cancelled. No database selected");
            return;
        }

        var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FilePath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

            var temp = await dropboxService.DownloadFileAsync(file);
            Log.Information("Downloading {FileName} from cloud storage", fileName);
            File.Move(temp, newFilePath, true);
            Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
        }
    }

    private static async Task ImportFromLocalAsync()
    {
        Log.Information("Starting to import the database from local storage");
        var dialog = new SqliteFileDialog(multiSelect: true);
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
        var itemsToDelete = ExistingDatabases
            .Where(s => !File.Exists(s.FilePath)).ToImmutableArray();

        foreach (var item in itemsToDelete)
        {
            ExistingDatabases.Remove(item);
        }

        var newExistingDatabases = DbContextBackup.GetExistingDatabase();
        foreach (var existingDatabase in newExistingDatabases)
        {
            var exist = ExistingDatabases.FirstOrDefault(s => s.FilePath == existingDatabase.FilePath);
            if (exist is not null)
            {
                existingDatabase.CopyPropertiesTo(exist);
            }
            else
            {
                ExistingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);
            }
        }

        _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync();
    }

    private static async Task SaveToCloudAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await ExportToCloudFileAsync(existingDatabasesSelected.First());
        else await ExportToCloudDirectoryAsync(existingDatabasesSelected);

        await existingDatabasesSelected.CheckExistingDatabaseIsSyncAsync();
    }

    private static void ShowSuccessMessage(string message)
        => MsgBox.Show(message, MsgBoxImage.Check, MessageBoxButton.OK);

    #endregion
}