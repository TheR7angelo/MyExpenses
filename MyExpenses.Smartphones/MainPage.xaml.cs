﻿using System.Collections.ObjectModel;
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
using MyExpenses.Smartphones.AppShells;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.ContentPages.SaveLocation;
using MyExpenses.Smartphones.UserControls.Buttons.UraniumButtonView;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
using Serilog;

namespace MyExpenses.Smartphones;

public partial class MainPage
{
    public ObservableCollection<ExistingDatabase> ExistingDatabases { get; } = [];

    public MainPage()
    {
        RefreshExistingDatabases();

        InitializeComponent();
    }

    #region Action

    private void ButtonAddDataBase_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddDataBase();

    private async void ButtonDatabase_OnClick(object? sender, EventArgs e)
    {
        var buttonImageView = (UraniumButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        if (existingDatabase.SyncStatus is SyncStatus.Unknown) existingDatabase.CheckExistingDatabaseIsSync(ProjectSystem.Maui);
        if (existingDatabase.SyncStatus is SyncStatus.LocalIsOutdated)
        {
            var question = string.Format(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionMessage, Environment.NewLine);

            var response = await DisplayAlert(WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionTitle, question,
                WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionYesButton, WelcomeManagementResources.MessageBoxUseOutdatedWarningQuestionCancelButton);
            if (response is not true) return;
        }

        var message = string.Format(WelcomeManagementResources.ActivityIndicatorOpenDatabase, existingDatabase.FileNameWithoutExtension);
        this.ShowCustomPopupActivityIndicator(message);

        DataBaseContext.FilePath = existingDatabase.FilePath;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Create a new instance of DashBoardShell and assign the selected database.
        // This action allocates memory for the new shell object and switches the application to the new shell.
        var dashBoardShell = new DashBoardShell { SelectedDatabase = existingDatabase };
        Application.Current!.Windows[0].Page = dashBoardShell;
    }

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private void ButtonExportDataBase_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonExportDataBase();

    private void ButtonImportDataBase_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonImportDataBase();

    private void ButtonRemoveDataBase_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonRemoveDataBase();

    #endregion

    #region Function

    private async Task<bool> ConfirmCloudDeletion()
    {
        return await DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionYesButton,
            WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionNoButton);
    }

    private async Task<bool> ConfirmLocalDeletion()
    {
        return await DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionYesButton,
            WelcomeManagementResources.MessageBoxRemoveDataBaseQuestionCancelButton);
    }

    private static async Task DeleteCloudFilesAsync(List<ExistingDatabase> databasesToDelete)
    {
        var files = databasesToDelete.Select(db => db.FileName).ToArray();
        Log.Information("Preparing to delete the following files: {Files}", files);

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
        _ = await dropboxService.DeleteFilesAsync(files, DatabaseInfos.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from Dropbox");
    }

    private void DeleteLocalDatabases(List<ExistingDatabase> databasesToDelete)
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

        RefreshExistingDatabases();
    }

    private async Task DisplaySuccessMessage()
    {
        await DisplayAlert(WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessTitle,
            WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessMessage,
            WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessOkButton);
    }

    private async Task ExportToCloudAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToCloud);
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

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private async Task ExportToLocalDatabase(List<ExistingDatabase> existingDatabasesSelected)
    {
        var folderPickerResult = await FolderPicker.Default.PickAsync();
        if (!folderPickerResult.IsSuccessful) return;

        var selectedFolder = folderPickerResult.Folder.Path;

        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal);

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

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private async Task<List<ExistingDatabase>?> ExportToLocalFolderAsync(List<ExistingDatabase> existingDatabasesSelected, bool isCompress)
    {
        var folderPickerResult = await FolderPicker.Default.PickAsync();
        if (!folderPickerResult.IsSuccessful) return null;

        var selectedFolder = folderPickerResult.Folder.Path;

        Log.Information("Starting to export database to {SelectedDialog}", selectedFolder);

        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorExportDatabaseToLocal);

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

    private async Task HandleButtonAddDataBase()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a new page of type AddDatabaseFileContentPage, likely used to manage or display database file content.
        var addDatabaseFileContentPage = new AddDatabaseFileContentPage();
        addDatabaseFileContentPage.SetExistingDatabase(ExistingDatabases);

        await addDatabaseFileContentPage.NavigateToAsync();

        var result = await addDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorCreateNewDatabase);

        var fileName = addDatabaseFileContentPage.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DatabaseInfos.LocalFilePathDataBaseModel, filePath, true);


            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Asynchronously initializes a new instance of DataBaseContext for interacting with the database at the specified file path.
            await using var context = new DataBaseContext(filePath);

            _ = context.SetAllDefaultValues();
            await context.SaveChangesAsync();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Adds a new instance of ExistingDatabase, created with the specified file path, to the ExistingDatabases collection
            // and sorts it based on the FileNameWithoutExtension property.
            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Information("New database was successfully added");
            await DisplayAlert(WelcomeManagementResources.MessageBoxAddDataBaseSuccessTitle, WelcomeManagementResources.MessageBoxAddDataBaseSuccessMessage, WelcomeManagementResources.MessageBoxAddDataBaseOkButton);
        }
        catch (Exception exception)
        {
            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Error(exception, "An error occur");

            await DisplayAlert(WelcomeManagementResources.MessageBoxAddDataBaseErrorTitle, WelcomeManagementResources.MessageBoxAddDataBaseErrorMessage, WelcomeManagementResources.MessageBoxAddDataBaseOkButton);
        }
    }

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private async Task HandleButtonExportDataBase()
    {
        var saveLocation = await SaveLocationContentPageUtils.GetExportSaveLocation();
        if (saveLocation is null) return;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a new page of type SelectDatabaseFileContentPage, likely used to allow the user to select database file content.
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();

        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);
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
                    await ExportToLocalDatabase(selectDatabaseFileContentPage.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Folder:
                    errors = await ExportToLocalFolderAsync(selectDatabaseFileContentPage.ExistingDatabasesSelected, false);
                    break;


                case SaveLocation.Dropbox:
                    await ExportToCloudAsync(selectDatabaseFileContentPage.ExistingDatabasesSelected);
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
                await DisplayAlert(
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseTitle,
                    message,
                    WelcomeManagementResources.MessageBoxExportDataBaseExportErrorSomeDatabaseOkButton);
            }
            else await DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseSuccessTitle, WelcomeManagementResources.MessageBoxExportDataBaseSuccessMessage, WelcomeManagementResources.MessageBoxExportDataBaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await DisplayAlert(WelcomeManagementResources.MessageBoxExportDataBaseErrorTitle, WelcomeManagementResources.MessageBoxExportDataBaseErrorMessage, WelcomeManagementResources.MessageBoxExportDataBaseErrorOkButton);
        }
    }

    private async Task HandleButtonImportDataBase()
    {
        var saveLocation = await SaveLocationMode.LocalDropbox.GetImportSaveLocation();
        if (saveLocation is null) return;

        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    await ImportFromLocalAsync(this);
                    break;
                case SaveLocation.Dropbox:
                    await ImportFromCloudAsync();
                    break;
                case SaveLocation.Folder:
                case SaveLocation.Database:
                case SaveLocation.Compress:
                case null:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RefreshExistingDatabases();

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            await DisplayAlert(WelcomeManagementResources.MessageBoxImportDatabaseSuccessTitle, WelcomeManagementResources.MessageBoxImportDatabaseSuccessMessage, WelcomeManagementResources.MessageBoxImportDatabaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await DisplayAlert(WelcomeManagementResources.MessageBoxImportDatabaseErrorTitle, WelcomeManagementResources.MessageBoxImportDatabaseErrorMessage, WelcomeManagementResources.MessageBoxImportDatabaseErrorOkButton);
        }
    }

    private async Task HandleButtonRemoveDataBase()
    {
        var databasesToDelete = await SelectDatabases();
        if (databasesToDelete is null || databasesToDelete.Count is 0) return;

        var confirmLocalDeletion = await ConfirmLocalDeletion();
        if (!confirmLocalDeletion) return;

        DeleteLocalDatabases(databasesToDelete);

        var confirmCloudDeletion = await ConfirmCloudDeletion();
        if (confirmCloudDeletion)
        {
            await DeleteCloudFilesAsync(databasesToDelete);
        }

        await DisplaySuccessMessage();
    }

    private async Task ImportFromCloudAsync()
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

        this.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorImportDatabaseFromCloud);
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

    private static async Task ImportFromLocalAsync(MainPage mainPage)
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

        mainPage.ShowCustomPopupActivityIndicator(WelcomeManagementResources.ActivityIndicatorImportDatabaseFromLocal);
        var filePath = result.FullPath;

        var fileName = Path.GetFileName(filePath);
        var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

        Log.Information("Copying {FileName} to local storage", fileName);
        File.Copy(filePath, newFilePath, true);
        Log.Information("Successfully copied {FileName} to local storage", fileName);
    }

    private void RefreshExistingDatabases()
    {
        var itemsToDelete = ExistingDatabases
            .Where(s => !File.Exists(s.FilePath)).ToArray();

        foreach (var item in itemsToDelete)
        {
            ExistingDatabases.Remove(item);
        }

        var newExistingDatabases = DbContextBackup.GetExistingDatabase();

        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var existingDatabase in newExistingDatabases)
        {
            // ReSharper disable once HeapView.DelegateAllocation
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

        _ = ExistingDatabases.CheckExistingDatabaseIsSyncAsync(ProjectSystem.Maui);
    }

    private async Task<List<ExistingDatabase>?> SelectDatabases()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Instantiates a SelectDatabaseFileContentPage, likely representing a page for selecting database file content.
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();

        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);
        await selectDatabaseFileContentPage.NavigateToAsync();

        var result = await selectDatabaseFileContentPage.ResultDialog;
        return result ? selectDatabaseFileContentPage.ExistingDatabasesSelected : null;
    }

    #endregion
}