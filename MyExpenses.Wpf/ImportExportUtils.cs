using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MyExpenses.Core;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.WebApi.Dropbox;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using MyExpenses.Wpf.Windows.SaveLocationWindow;
using Serilog;

namespace MyExpenses.Wpf;

public static class ImportExportUtils
{
    /// <summary>
    /// Retrieves the list of databases selected by the user from a modal window.
    /// This method displays a selection window for the user to choose from an existing list of databases.
    /// </summary>
    /// <param name="existingDatabases">The collection of existing databases that will be displayed in the selection window for the user to choose from.</param>
    /// <returns>A list of selected databases if the selection is confirmed by the user; otherwise, null if no selection is made or the operation is canceled.</returns>
    public static List<ExistingDatabase>? GetSelectedDatabases(this IEnumerable<ExistingDatabase> existingDatabases)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SelectDatabaseFileWindow is created to handle the selection of existing databases to remove.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabases);
        selectDatabaseFileWindow.ShowDialog();

        return selectDatabaseFileWindow.DialogResult == true
            ? selectDatabaseFileWindow.ExistingDatabasesSelected
            : null;
    }

    #region Delete

    /// <summary>
    /// Handles the removal of selected databases from both local storage and the cloud, if confirmed by the user.
    /// This method interacts with the user through confirmation dialogs and performs the necessary actions
    /// based on the user's input.
    /// </summary>
    /// <param name="existingDatabases">The collection of existing databases to be managed and potentially removed.</param>
    /// <returns>A task representing the asynchronous operation of database removal.</returns>
    public static async Task HandleButtonRemoveDataBase(this ObservableCollection<ExistingDatabase> existingDatabases)
    {
        var selectedDatabases = existingDatabases.GetSelectedDatabases();
        if (selectedDatabases is null || selectedDatabases.Count is 0) return;

        var confirmLocalDelection = MsgBox.Show(WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionMessage,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (confirmLocalDelection is not MessageBoxResult.Yes) return;

        selectedDatabases.DeleteLocalDatabases();
        existingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

        var confirmCloudDeletion = MsgBox.Show(WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionMessage,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (confirmCloudDeletion is not MessageBoxResult.Yes) return;

        await selectedDatabases.DeleteCloudFilesAsync(ProjectSystem.Wpf);

        MsgBox.Show(WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessMessage, MsgBoxImage.Check, MessageBoxButton.OK);
    }

    #endregion

    #region Export

    /// <summary>
    /// Exports the selected database to a specified local file path on the system.
    /// The method provides a file dialog for the user to select the save location and name for the database file.
    /// </summary>
    /// <param name="existingDatabasesSelected">The existing database instance containing the file path and filename of the database to be exported.</param>
    /// <returns>A task that represents the asynchronous operation of exporting the database file.</returns>
    public static Task ExportToLocalDatabaseFileAsync(this ExistingDatabase existingDatabasesSelected)
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

    public static Task ExportToLocalDirectoryDatabaseAsync(this List<ExistingDatabase> existingDatabasesSelected)
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

    /// <summary>
    /// Exports the selected databases to a specified folder on the local system.
    /// The method allows for optional compression during the export process and provides user feedback on success or failure.
    /// </summary>
    /// <param name="existingDatabasesSelected">A list of existing database instances to be exported.</param>
    /// <param name="isCompress">A boolean value indicating whether the exported files should be compressed.</param>
    /// <returns>A task that represents the asynchronous operation of exporting the selected databases to a local folder.</returns>
    public static async Task ExportToLocalFolderAsync(this List<ExistingDatabase> existingDatabasesSelected,
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

    /// <summary>
    /// Handles the export process of the selected database based on the chosen save location.
    /// The method includes displaying selection dialogs, handling export actions, and providing feedback to the user.
    /// </summary>
    /// <param name="existingDatabases">A list of existing database instances available for selection and export.</param>
    /// <returns>A task that represents the asynchronous operation of exporting the selected database.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided save location is invalid or unsupported.</exception>
    public static async Task HandleButtonExportDataBase(this IEnumerable<ExistingDatabase> existingDatabases)
    {
        var saveLocation = SaveLocationUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of SelectDatabaseFileWindow is created to handle the selection of existing databases to export.
        // The SetExistingDatabase method is called with the ExistingDatabases to provide context or validate against existing entries.
        // ShowDialog() is used to display the window modally and obtain the user's action.
        // If the dialog result is not true (e.g., the user cancels or closes the window), the method exits early.
        var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
        selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabases);

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
                    await selectDatabaseFileWindow.ExistingDatabasesSelected.SaveToLocalDatabase();
                    break;

                case SaveLocation.Folder:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await selectDatabaseFileWindow.ExistingDatabasesSelected.ExportToLocalFolderAsync(false);
                    break;

                case SaveLocation.Compress:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await selectDatabaseFileWindow.ExistingDatabasesSelected.ExportToLocalFolderAsync(true);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToCloud;
                    waitScreenWindow.Show();
                    await selectDatabaseFileWindow.ExistingDatabasesSelected.SaveToCloudAsync(ProjectSystem.Wpf);
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

    /// <summary>
    /// Handles the export operation for a specified database file path, allowing users to export the database
    /// to a selected save location. The method supports various export options like saving to local storage,
    /// compressing the file, or uploading to the cloud.
    /// </summary>
    /// <param name="databaseFilePath">The file path of the database to be exported.</param>
    /// <returns>A Task representing the asynchronous export operation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported save location option is selected.</exception>
    public static async Task HandleButtonExportDataBase(this string databaseFilePath)
    {
        var existingDatabase = new ExistingDatabase(databaseFilePath);
        var existingDatabases = new List<ExistingDatabase> {existingDatabase};

        var saveLocation = SaveLocationUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

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
                    await existingDatabases.SaveToLocalDatabase();
                    break;

                case SaveLocation.Folder:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await existingDatabases.ExportToLocalFolderAsync(false);
                    break;

                case SaveLocation.Compress:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal;
                    waitScreenWindow.Show();
                    await existingDatabases.ExportToLocalFolderAsync(true);
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorExportDatabaseToCloud;
                    waitScreenWindow.Show();
                    await existingDatabases.SaveToCloudAsync(ProjectSystem.Wpf);
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

    /// <summary>
    /// Saves the selected databases to a local file or directory, depending on the number of databases provided.
    /// If a single database is provided, it is saved to a file.
    /// If multiple databases are provided, they're saved to a directory.
    /// </summary>
    /// <param name="existingDatabasesSelected">A list of existing database instances that represent the databases to be saved locally.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public static async Task SaveToLocalDatabase(this List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await existingDatabasesSelected.First().ExportToLocalDatabaseFileAsync();
        else await existingDatabasesSelected.ExportToLocalDirectoryDatabaseAsync();
    }

    #endregion

    #region Import

    public static async Task ImportFromCloudAsync()
    {
        Log.Information("Starting to import the database from cloud storage");
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        var metadatas = await dropboxService.ListFileAsync(DatabaseInfos.CloudDirectoryBackupDatabase);
        metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DatabaseInfos.Extension));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of ExistingDatabase is created for each file in the cloud directory.
        var existingDatabases = metadatas.Select(s => new ExistingDatabase(s.PathDisplay)).ToArray();
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
            var response = MsgBox.Show(WelcomeManagementResources.CloudDatabaseOutdatedWarningQuestionTitle, question,
                MessageBoxButton.YesNoCancel, MsgBoxImage.Warning);
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

    /// <summary>
    /// Imports one or more SQLite database files from the local storage into the application's designated local directory.
    /// The method uses a file dialog to allow the user to select the desired database files for the operation.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of importing and copying the database files to the local directory.</returns>
    public static async Task ImportFromLocalAsync()
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

    public static async Task HandleButtonImportDataBase(this ObservableCollection<ExistingDatabase> existingDatabases)
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
                    await ImportExportUtils.ImportFromLocalAsync();
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomeManagementResources.ActivityIndicatorImportDatabaseFromCloud;
                    waitScreenWindow.Show();
                    await ImportExportUtils.ImportFromCloudAsync();
                    break;

                case null:
                case SaveLocation.Folder:
                case SaveLocation.Database:
                case SaveLocation.Compress:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            existingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

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

    #endregion
}