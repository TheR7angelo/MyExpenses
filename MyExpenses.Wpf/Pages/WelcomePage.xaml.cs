using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
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

        // ReSharper disable once HeapView.DelegateAllocation
        MainWindow.VaccumDatabase += MainWindow_OnVaccumDatabase;
    }

    private void MainWindow_OnVaccumDatabase()
        => _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Wpf);

    #region Action

    private void ButtonAddDataBase_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddDatabaseFileWindow is created to handle the addition of a new database file.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var addDatabaseFileWindow = new AddDatabaseFileWindow();
        addDatabaseFileWindow.SetExistingDatabase(ExistingDatabases);

        var result = addDatabaseFileWindow.ShowDialog();

        if (result is not true) return;

        var fileName = addDatabaseFileWindow.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, DatabaseInfos.Extension);
        var filePath = Path.Combine(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DatabaseInfos.LocalFilePathDataBaseModel, filePath, true);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Necessary instantiation of DataBaseContext to interact with the database.
            // This creates a scoped database context for performing queries and modifications in the database.
            using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            context.SaveChanges();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The instantiation of ExistingDatabase is necessary to interact with
            // a specific database identified by its file path (filePath).
            // This ensures that each database is represented by a dedicated instance,
            // allowing proper management and addition to the ExistingDatabases collection.
            var existingDatabase = new ExistingDatabase(filePath);
            ExistingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);

            Log.Information("New database was successfully added");
            MsgBox.Show(WelcomeManagementResources.MessageBoxAddDataBaseSuccessTitle,
                WelcomeManagementResources.MessageBoxAddDataBaseSuccessMessage, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur");
            MsgBox.Show(WelcomeManagementResources.MessageBoxAddDataBaseErrorTitle,
                WelcomeManagementResources.MessageBoxAddDataBaseErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonDatabase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not ExistingDatabase existingDatabase) return;

        if (existingDatabase.SyncStatus is SyncStatus.Unknown) existingDatabase.CheckExistingDatabaseIsSync(ProjectSystem.Wpf);
        if (existingDatabase.SyncStatus is SyncStatus.LocalIsOutdated)
        {
            var question = string.Format(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionMessage, Environment.NewLine);

            var response = MsgBox.Show(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionTitle, question, MessageBoxButton.YesNo, MsgBoxImage.Question);
            if (response is not MessageBoxResult.Yes) return;
        }

        Log.Information("Connection to the database : \"{FileName}\" with statut : {Status}", existingDatabase.FileNameWithoutExtension, existingDatabase.SyncStatus);

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
        _ = await dropboxService.DeleteFilesAsync(files, DatabaseInfos.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from cloud");
    }

    private static void DeleteLocalDatabases(List<ExistingDatabase> databases)
    {
        foreach (var database in databases.Where(database =>
                     !string.IsNullOrEmpty(database.FilePath) && File.Exists(database.FilePath)))
        {
            File.Delete(database.FilePath);

            var backupDirectory = Path.Join(DatabaseInfos.LocalDirectoryBackupDatabase,
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
                DatabaseInfos.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage",
                existingDatabase.FileName);
        }
    }

    private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
        _ = await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath,
            DatabaseInfos.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
    }

    private static async Task ExportToLocalFolderAsync(List<ExistingDatabase> existingDatabasesSelected,
        bool isCompress)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of FolderDialog is created to handle the selection of a folder to export the database to.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var folderDialog = new FolderDialog();
        var selectedDialog = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        Log.Information("Starting to export database to {SelectedDialog}", selectedDialog);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This code uses Parallel.ForEachAsync for parallel processing of database exports, maximizing performance by utilizing multiple threads.
        // A thread-safe ConcurrentBag is used to track failed exports. Logs provide feedback on success or failure for each database export.
        var failedExistingDatabases = new List<ExistingDatabase>();
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to export {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);

            var success = await existingDatabase.ToFolderAsync(selectedDialog, isCompress);

            if (!success)
            {
                failedExistingDatabases.Add(existingDatabase);
                Log.Warning("Failed to export {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
            }
            else
            {
                Log.Information("Successfully exported {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
            }
        }

        if (!failedExistingDatabases.Count.Equals(0))
        {
            Log.Information("Failed to export some database to local folder");
            var message = string.Format(WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseMessage, Environment.NewLine, string.Join(", ", failedExistingDatabases.Select(s => s.FileNameWithoutExtension)));
            MsgBox.Show(WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseTitle,
                message, MessageBoxButton.OK, MsgBoxImage.Error);
            return;
        }

        Log.Information("Database successfully copied to local storage");

        var response = MsgBox.Show(WelcomeManagementResources.MessageBoxOpenExportFolderQuestionMessage, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedDialog.StartFile();
    }

    private List<ExistingDatabase>? GetSelectedDatabases()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SelectDatabaseFileWindow is created to handle the selection of existing databases to remove.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SelectDatabaseFileWindow is created to handle the selection of existing databases to export.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(ExistingDatabases);

        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true) return;
        if (selectDatabaseFileWindow.ExistingDatabasesSelected.Count.Equals(0)) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of WaitScreenWindow is created to handle the display of a wait screen while the export is in progress.
        // The Show() method is used to display the window and start the wait screen.
        // The Close() method is used to close the window and stop the wait screen.
        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await SaveToLocalDatabase(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Folder:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(selectDatabaseFileWindow.ExistingDatabasesSelected, false);
                    break;

                case SaveLocation.Compress:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await ExportToLocalFolderAsync(selectDatabaseFileWindow.ExistingDatabasesSelected, true);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToCloud;
                    waitScreenWindow.Show();
                    await SaveToCloudAsync(selectDatabaseFileWindow.ExistingDatabasesSelected);
                    break;

                case null:
                case SaveLocation.Local:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            waitScreenWindow.Close();

            MsgBox.Show(WelcomeManagementResources.ButtonExportDataBaseSuccessMessage, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(WelcomeManagementResources.MessageBoxExportDataBaseErrorMessage, MsgBoxImage.Warning);
        }
    }

    private async Task HandleButtonImportDataBase()
    {
        var saveLocation = SaveLocationUtils.GetImportSaveLocation(SaveLocationMode.LocalDropbox);
        if (saveLocation is null) return;

        // An instance of AddDatabaseFileWindow is created to handle the addition of a new database file.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and get the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var waitScreenWindow = new WaitScreenWindow();
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorImportDatabaseFromLocal;
                    waitScreenWindow.Show();
                    await ImportFromLocalAsync();
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorImportDatabaseFromCloud;
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
            MsgBox.Show(WelcomeManagementResources.ButtonImportDataBaseImportSucessMessage, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(WelcomeManagementResources.ButtonImportDataBaseErrorMessage, MsgBoxImage.Warning);
        }
    }

    private async Task HandleButtonRemoveDataBase()
    {
        var selectedDatabases = GetSelectedDatabases();
        if (selectedDatabases is null || selectedDatabases.Count is 0) return;

        var confirmLocalDeletion = ConfirmDeletion(WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionMessage);
        if (!confirmLocalDeletion) return;

        DeleteLocalDatabases(selectedDatabases);
        RefreshExistingDatabases();

        var confirmCloudDeletion = ConfirmDeletion(WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionMessage);
        if (!confirmCloudDeletion) return;

        await DeleteCloudFilesAsync(selectedDatabases);

        ShowSuccessMessage(WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessMessage);
    }

    private static async Task SaveToLocalDatabase(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1)
            await ExportToLocalDatabaseFileAsync(existingDatabasesSelected.First());
        else await ExportToLocalDirectoryDatabaseAsync(existingDatabasesSelected);
    }

    private static Task ExportToLocalDirectoryDatabaseAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of FolderDialog is created to handle the selection of a folder to export the database to.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var folderDialog = new FolderDialog();
        var selectedFolder = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedFolder))
        {
            Log.Warning("Export cancelled. No directory selected");
            return Task.CompletedTask;
        }

        foreach (var existingDatabase in existingDatabasesSelected)
        {
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);

            File.Copy(existingDatabase.FilePath, newFilePath, true);
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}",
                existingDatabase.FileName, newFilePath);
        }

        var response = MsgBox.Show(WelcomeManagementResources.MessageBoxOpenExportFolderQuestionMessage, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedFolder.StartFile();

        return Task.CompletedTask;
    }

    private static Task ExportToLocalDatabaseFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SqliteFileDialog is created to handle the selection of a file to export the database to.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var sqliteDialog = new SqliteFileDialog(defaultFileName: existingDatabasesSelected.FileName);
        var selectedDialog = sqliteDialog.SaveFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return Task.CompletedTask;;
        }

        selectedDialog = Path.ChangeExtension(selectedDialog, DatabaseInfos.Extension);
        var selectedFilePath = existingDatabasesSelected.FilePath;
        Log.Information("Starting to copy database to {SelectedDialog}", selectedDialog);

        File.Copy(selectedFilePath, selectedDialog, true);
        Log.Information("Database successfully copied to local storage");

        var parentDirectory = Path.GetDirectoryName(selectedDialog)!;
        var response = MsgBox.Show(WelcomeManagementResources.MessageBoxOpenExportFolderQuestionMessage, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) parentDirectory.StartFile();

        return Task.CompletedTask;
    }

    private static async Task ImportFromCloudAsync()
    {
        Log.Information("Starting to import the database from cloud storage");
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        var metadatas = await dropboxService.ListFileAsync(DatabaseInfos.CloudDirectoryBackupDatabase);
        metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DatabaseInfos.Extension));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of ExistingDatabase is created for each file in the cloud directory.
        var existingDatabases = metadatas.Select(s => new ExistingDatabase(s.PathDisplay)).ToList();
        foreach (var existingDatabase in existingDatabases)
        {
            var filePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, existingDatabase.FileName);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // An instance of ExistingDatabase is created to handle the status of the database.
            var localDatabase = new ExistingDatabase(filePath);
            existingDatabase.SyncStatus = await localDatabase.CheckStatus(ProjectSystem.Wpf);
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SelectDatabaseFileWindow is created to handle the selection of existing databases to import.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabases);
        selectDatabaseFileWindow.ShowDialog();

        if (selectDatabaseFileWindow.DialogResult is not true)
        {
            Log.Warning("Import cancelled. No database selected");
            return;
        }

        if (selectDatabaseFileWindow.ExistingDatabasesSelected.Any(s => s.SyncStatus is SyncStatus.RemoteIsOutdated))
        {
            var question = string.Format(WelcomeManagementResources.CloudDatabaseOutdatedWarningQuestionMessage, Environment.NewLine);
            var response = MsgBox.Show(question, MsgBoxImage.Warning, MessageBoxButton.YesNo);
            if (response is not MessageBoxResult.Yes)
            {
                Log.Information("Import cancelled. User chose to not import the cloud databases");
                return;
            }
        }

        var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FilePath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

            var temp = await dropboxService.DownloadFileAsync(file);
            Log.Information("Downloading {FileName} from cloud storage", fileName);
            File.Move(temp, newFilePath, true);
            Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
        }
    }

    private static async Task ImportFromLocalAsync()
    {
        Log.Information("Starting to import the database from local storage");

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SqliteFileDialog is created to handle the selection of a file to import the database from.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
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
            var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

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
        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var existingDatabase in newExistingDatabases)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var exist = ExistingDatabases.FirstOrDefault(s => s.FilePath.Equals(existingDatabase.FilePath));
            if (exist is not null)
            {
                existingDatabase.CopyPropertiesTo(exist);
            }
            else
            {
                ExistingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);
            }
        }

        _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Wpf);
    }

    private static async Task SaveToCloudAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await ExportToCloudFileAsync(existingDatabasesSelected.First());
        else await ExportToCloudDirectoryAsync(existingDatabasesSelected);

        await existingDatabasesSelected.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Wpf);
    }

    private static void ShowSuccessMessage(string message)
        => MsgBox.Show(message, MsgBoxImage.Check, MessageBoxButton.OK);

    #endregion
}