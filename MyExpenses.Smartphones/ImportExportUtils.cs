using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using CommunityToolkit.Maui.Storage;
using MyExpenses.Core.Export;
using MyExpenses.Maui.Utils.WebApi;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.ContentPages.SaveLocation;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
using Serilog;

namespace MyExpenses.Smartphones;

public static class ImportExportUtils
{
    /// <summary>
    /// Refreshes the collection of existing databases by removing entries that no longer exist on the file system
    /// and updating or adding databases from the current backup directory.
    /// </summary>
    /// <param name="existingDatabases">A collection of existing database entries to refresh and update.</param>
    public static void RefreshExistingDatabases(this ObservableCollection<ExistingDatabase> existingDatabases)
    {
        var itemsToDelete = existingDatabases
            .Where(s => !File.Exists(s.FilePath)).ToArray();

        foreach (var item in itemsToDelete)
        {
            existingDatabases.Remove(item);
        }

        var newExistingDatabases = DbContextBackup.GetExistingDatabase();

        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var existingDatabase in newExistingDatabases)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var exist = existingDatabases.FirstOrDefault(s => s.FilePath == existingDatabase.FilePath);
            if (exist is not null)
            {
                existingDatabase.CopyPropertiesTo(exist);
            }
            else
            {
                existingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);
            }
        }

        _ = existingDatabases.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Maui);
    }

    /// <summary>
    /// Displays a user interface for selecting databases from the provided collection of existing databases.
    /// Returns the selected databases or null if no selection was made.
    /// </summary>
    /// <param name="existingDatabases">A collection of existing databases to be presented for selection.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of selected databases,
    /// or null if no databases were selected.
    /// </returns>
    private static async Task<List<ExistingDatabase>?> SelectDatabases(this IEnumerable<ExistingDatabase> existingDatabases)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a SelectDatabaseFileContentPage, likely representing a page for selecting database file content.
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();

        selectDatabaseFileContentPage.ExistingDatabases.AddRange(existingDatabases);
        await selectDatabaseFileContentPage.NavigateToAsync();

        var result = await selectDatabaseFileContentPage.ResultDialog;
        return result ? selectDatabaseFileContentPage.ExistingDatabasesSelected : null;
    }

    #region Delete

    public static async Task HandleButtonRemoveDataBase(this Page parent, ObservableCollection<ExistingDatabase> existingDatabases)
    {
        var databasesToDelete = await existingDatabases.SelectDatabases();
        if (databasesToDelete is null || databasesToDelete.Count is 0) return;

        var confirmLocalDeletion = await parent.DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionYesButton,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionCancelButton);

        if (!confirmLocalDeletion) return;

        databasesToDelete.DeleteLocalDatabases();

        var confirmCloudDeletion = await parent.DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionYesButton,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionNoButton);

        if (confirmCloudDeletion)
        {
            await databasesToDelete.DeleteCloudFilesAsync();
        }

        await parent.DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessOkButton);

        existingDatabases.RefreshExistingDatabases();
    }

    /// <summary>
    /// Deletes a collection of cloud-stored database files from the specified cloud directory.
    /// </summary>
    /// <param name="databasesToDelete">A collection of database entries representing the files to be deleted from the cloud storage.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public static async Task DeleteCloudFilesAsync(this IEnumerable<ExistingDatabase> databasesToDelete)
    {
        var files = databasesToDelete.Select(db => db.FileName).ToArray();
        Log.Information("Preparing to delete the following files: {Files}", files);

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
        _ = await dropboxService.DeleteFilesAsync(files, DatabaseInfos.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from Dropbox");
    }

    /// <summary>
    /// Deletes the specified local databases and their corresponding backup directories
    /// from the local file system, if they exist.
    /// </summary>
    /// <param name="databasesToDelete">A collection of database entries to be deleted from the local file system.</param>
    public static void DeleteLocalDatabases(this IEnumerable<ExistingDatabase> databasesToDelete)
    {
        foreach (var database in databasesToDelete.Where(database => !string.IsNullOrEmpty(database.FilePath) && File.Exists(database.FilePath)))
        {
            File.Delete(database.FilePath);

            var backupDirectory = Path.Join(DatabaseInfos.LocalDirectoryBackupDatabase, database.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory))
            {
                Directory.Delete(backupDirectory, true);
            }
        }
    }

    #endregion

    #region Export

    /// <summary>
    /// Handles the action of exporting database files from the application.
    /// </summary>
    /// <param name="parent">The ContentPage instance initiating the export process.</param>
    /// <param name="existingDatabases">A collection of existing databases to be exported.</param>
    /// <returns>A task representing the asynchronous operation of exporting database files.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the selected save location is not supported.</exception>
    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    public static async Task HandleButtonExportDataBase(this Page parent, IEnumerable<ExistingDatabase> existingDatabases)
    {
        var saveLocation = await SaveLocationContentPageUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a new page of type SelectDatabaseFileContentPage, likely used to allow the user to select database file content.
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();

        selectDatabaseFileContentPage.ExistingDatabases.AddRange(existingDatabases);
        await selectDatabaseFileContentPage.NavigateToAsync();
        var result = await selectDatabaseFileContentPage.ResultDialog;
        if (result is not true) return;
        if (selectDatabaseFileContentPage.ExistingDatabasesSelected.Count.Equals(0)) return;

        List<ExistingDatabase>? errors = null;
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    await parent.ExportToLocalDatabase(selectDatabaseFileContentPage.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Folder:
                    errors = await parent.ExportToLocalFolderAsync(selectDatabaseFileContentPage.ExistingDatabasesSelected, false);
                    break;


                case SaveLocation.Dropbox:
                    await parent.ExportToCloudAsync(selectDatabaseFileContentPage.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Local:
                case SaveLocation.Compress:
                case null:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (errors is {Count: > 0})
            {
                var message = string.Format(WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseMessage, Environment.NewLine, string.Join(", ", errors.Select(s => s.FileNameWithoutExtension)));
                await parent.DisplayAlert(
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseTitle,
                    message,
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseOkButton);
            }
            else await parent.DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseSuccessTitle, WelcomeManagementResources.MessageBoxExportDataBaseSuccessMessage, WelcomeManagementResources.MessageBoxExportDataBaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await parent.DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseErrorTitle, WelcomeManagementResources.MessageBoxExportDataBaseErrorMessage, WelcomeManagementResources.MessageBoxExportDataBaseErrorOkButton);
        }
    }

    /// <summary>
    /// Handles the export of a database by allowing the user to select a save location
    /// and performing the export operation to the chosen destination.
    /// </summary>
    /// <param name="parent">The page from which the export operation is initiated, used for UI-related operations.</param>
    /// <param name="existingDatabase">The databases to be exported.</param>
    /// <returns>A task representing the asynchronous export operation.</returns>
    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    public static async Task HandleButtonExportDataBase(this Page parent, ExistingDatabase existingDatabase)
    {
        var exportList = new List<ExistingDatabase> { existingDatabase };
        var saveLocation = await SaveLocationContentPageUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        List<ExistingDatabase>? errors = null;
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    await parent.ExportToLocalDatabase(exportList);
                    break;

                case SaveLocation.Folder:
                    errors = await parent.ExportToLocalFolderAsync(exportList, false);
                    break;


                case SaveLocation.Dropbox:
                    await parent.ExportToCloudAsync(exportList);
                    break;

                case SaveLocation.Local:
                case SaveLocation.Compress:
                case null:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (errors is {Count: > 0})
            {
                var message = string.Format(WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseMessage, Environment.NewLine, string.Join(", ", errors.Select(s => s.FileNameWithoutExtension)));
                await parent.DisplayAlert(
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseTitle,
                    message,
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseOkButton);
            }
            else await parent.DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseSuccessTitle, WelcomeManagementResources.MessageBoxExportDataBaseSuccessMessage, WelcomeManagementResources.MessageBoxExportDataBaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await parent.DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseErrorTitle, WelcomeManagementResources.MessageBoxExportDataBaseErrorMessage, WelcomeManagementResources.MessageBoxExportDataBaseErrorOkButton);
        }
    }

    /// <summary>
    /// Exports the selected databases to a cloud storage service asynchronously.
    /// </summary>
    /// <param name="parent">The parent ContentPage that triggers the export operation.</param>
    /// <param name="existingDatabasesSelected">The list of databases selected for export to the cloud.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    public static async Task ExportToCloudAsync(this Page parent,
        List<ExistingDatabase> existingDatabasesSelected)
    {
        parent.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToCloud);
        Log.Information("Starting to export database to cloud storage");

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
            _ = await dropboxService.UploadFileAsync(existingDatabase.FilePath, DatabaseInfos.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
        }
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
    }

    /// <summary>
    /// Exports the selected databases to a specified local folder.
    /// </summary>
    /// <param name="parent">The parent ContentPage that initiates the export process.</param>
    /// <param name="existingDatabasesSelected">The list of databases selected for export.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    public static async Task ExportToLocalDatabase(this Page parent, List<ExistingDatabase> existingDatabasesSelected)
    {
        var folderPickerResult = await FolderPicker.Default.PickAsync();
        if (!folderPickerResult.IsSuccessful) return;

        var selectedFolder = folderPickerResult.Folder.Path;

        parent.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal);

        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            // ReSharper disable once HeapView.ClosureAllocation
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);

            // ReSharper disable once HeapView.DelegateAllocation
            await Task.Run(() => File.Copy(existingDatabase.FilePath, newFilePath, true));
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
        }
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
    }

    /// <summary>
    /// Exports the selected databases to a local folder, with an option to compress them.
    /// </summary>
    /// <param name="parent">The parent ContentPage that initiates the export process.</param>
    /// <param name="existingDatabasesSelected">The list of databases selected for export.</param>
    /// <param name="isCompress">A flag indicating whether the databases should be compressed during export.</param>
    /// <returns>A list of databases that failed to export, or null if the operation was canceled or fully successful.</returns>
    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    public static async Task<List<ExistingDatabase>?> ExportToLocalFolderAsync(this Page parent, List<ExistingDatabase> existingDatabasesSelected, bool isCompress)
    {
        var folderPickerResult = await FolderPicker.Default.PickAsync();
        if (!folderPickerResult.IsSuccessful) return null;

        var selectedFolder = folderPickerResult.Folder.Path;

        Log.Information("Starting to export database to {SelectedDialog}", selectedFolder);

        parent.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal);

        List<ExistingDatabase>? failedExistingDatabases = null;
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to export {ExistingDatabaseFileName} to {SelectedDialog}", existingDatabase.FileNameWithoutExtension, selectedFolder);
            var success = await existingDatabase.ToFolderAsync(selectedFolder, isCompress);
            if (!success)
            {
                failedExistingDatabases ??= [];
                failedExistingDatabases.Add(existingDatabase);
                Log.Error("Failed to export {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
            }
            else Log.Information("Successfully exported {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
        }

        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        var maxFailedDatabase = failedExistingDatabases?.Count ?? 0;
        var rate = existingDatabasesSelected.Count - maxFailedDatabase;
        Log.Information("Exporting database to {SelectedDialog} completed with {Rate}/{ExistingDatabasesSelectedCount}", selectedFolder, rate, existingDatabasesSelected.Count);

        return failedExistingDatabases;
    }

    #endregion

    #region Import

    public static async Task HandleButtonImportDataBase(this Page parent, ObservableCollection<ExistingDatabase> existingDatabases)
    {
        var saveLocation = await SaveLocationMode.LocalDropbox.GetImportSaveLocation();
        if (saveLocation is null) return;

        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    await parent.ImportFromLocalAsync();
                    break;
                case SaveLocation.Dropbox:
                    await parent.ImportFromCloudAsync();
                    break;
                case SaveLocation.Folder:
                case SaveLocation.Database:
                case SaveLocation.Compress:
                case null:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExistingDatabases(existingDatabases);

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            await parent.DisplayAlert(WelcomeManagementResources.MessageBoxImportDatabaseSuccessTitle, WelcomeManagementResources.MessageBoxImportDatabaseSuccessMessage, WelcomeManagementResources.MessageBoxImportDatabaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await parent.DisplayAlert(WelcomeManagementResources.MessageBoxImportDatabaseErrorTitle, WelcomeManagementResources.MessageBoxImportDatabaseErrorMessage, WelcomeManagementResources.MessageBoxImportDatabaseErrorOkButton);
        }
    }

    /// <summary>
    /// Imports a database file from cloud storage into the application's designated directory, allowing the user to select a desired file.
    /// </summary>
    /// <param name="parent">The ContentPage instance initiating the import process, used for displaying dialogs and activity indicators.</param>
    /// <returns>A task representing the asynchronous operation of importing a database file from cloud storage.</returns>
    /// <exception cref="IOException">Thrown if an error occurs during the download or file transfer process.</exception>
    /// <exception cref="Exception">Thrown if an unexpected error occurs during the operation.</exception>
    public static async Task ImportFromCloudAsync(this Page parent)
    {
        Log.Information("Starting to import the database from cloud storage");
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
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
            existingDatabase.SyncStatus = await localDatabase.CheckStatus(ProjectSystem.Maui);
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Creates an instance of SelectDatabaseFileContentPage, likely used to handle the selection of database file content.
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(existingDatabases);

        await selectDatabaseFileContentPage.NavigateToAsync();

        var result = await selectDatabaseFileContentPage.ResultDialog;

        if (result is not true)
        {
            Log.Warning("Import cancelled. No database selected");
            return;
        }

        if (selectDatabaseFileContentPage.ExistingDatabasesSelected.Any(s => s.SyncStatus is SyncStatus.RemoteIsOutdated))
        {
            var question = string.Format(WelcomeManagementResources.CloudDatabaseOutdatedWarningQuestionMessage, Environment.NewLine);
            var response = await parent.DisplayAlert("Question", question, "Yes", "No");
            if (response is not true)
            {
                Log.Information("Import cancelled. User chose to not import the cloud databases");
                return;
            }
        }

        parent.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorImportDatabaseFromCloud);
        var mauiClient = HttpClientHandlerCustom.CreateHttpClientHandler();
        var files = selectDatabaseFileContentPage.ExistingDatabasesSelected.Select(s => s.FilePath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

            var fileTemp = Path.Join(AppContext.BaseDirectory, "temp.sqlite");

            Log.Information("Downloading {FileName} from cloud storage", fileName);
            var temp = await dropboxService.DownloadFileAsync(file, fileTemp, mauiClient);

            File.Move(temp, newFilePath, true);
            Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
        }
    }

    /// <summary>
    /// Imports a database file from the local storage into the application's designated directory.
    /// </summary>
    /// <param name="parent">The ContentPage instance initiating the import process.</param>
    /// <returns>A task representing the asynchronous operation of importing a database file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the chosen file can't be found.</exception>
    /// <exception cref="IOException">Thrown if an error occurs while copying the file.</exception>
    public static async Task ImportFromLocalAsync(this Page parent)
    {
        Log.Information("Starting to import the database from local storage");

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Defines a dictionary mapping each DevicePlatform to its associated collection of MIME types or UTIs for handling file types.
        var dictionary = new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, ["public.database"] },
            { DevicePlatform.Android, ["application/octet-stream"] },
            { DevicePlatform.MacCatalyst, ["public.database"] }
        };

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // Creates a new PickOptions instance to specify file picker options, including allowed FileTypes using the provided dictionary.
        var filePickerOption = new PickOptions { FileTypes = new FilePickerFileType(dictionary) };
        // ReSharper restore HeapView.ObjectAllocation.Evident

        var result = await FilePicker.PickAsync(filePickerOption);
        if (result is null) return;

        parent.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorImportDatabaseFromLocal);
        var filePath = result.FullPath;

        var fileName = Path.GetFileName(filePath);
        var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Copying {FileName} to local storage", fileName);
        File.Copy(filePath, newFilePath, true);
        Log.Information("Successfully copied {FileName} to local storage", fileName);
    }

    #endregion
}