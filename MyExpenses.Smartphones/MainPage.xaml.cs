using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using CommunityToolkit.Maui.Storage;
using MyExpenses.Core.Export;
using MyExpenses.Maui.Utils.WebApi;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Smartphones.AppShells;
using MyExpenses.Smartphones.ContentPages;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.ContentPages.SaveLocation;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.MainPage;
using MyExpenses.Smartphones.UserControls.Buttons.CustomFrame;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
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

    private void ButtonDatabase_OnClick(object? sender, EventArgs e)
    {
        var buttonImageView = (ButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        var message = string.Format(MainPageResources.CustomPopupActivityIndicatorOpenDatabase, existingDatabase.FileNameWithoutExtension);
        this.ShowCustomPopupActivityIndicator(message);

        DataBaseContext.FilePath = existingDatabase.FilePath;

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
        return await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionTitle,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionMessage,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionYesButton,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionNoButton);
    }

    private async Task<bool> ConfirmLocalDeletion()
    {
        return await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseQuestionTitle,
            MainPageResources.MessageBoxRemoveDataBaseQuestionMessage,
            MainPageResources.MessageBoxRemoveDataBaseQuestionYesButton,
            MainPageResources.MessageBoxRemoveDataBaseQuestionCancelButton);
    }

    private static async Task DeleteCloudFilesAsync(List<ExistingDatabase> databasesToDelete)
    {
        var files = databasesToDelete.Select(db => db.FileName).ToArray();
        Log.Information("Preparing to delete the following files: {Files}", files);

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
        _ = await dropboxService.DeleteFilesAsync(files, DbContextBackup.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from Dropbox");
    }

    private void DeleteLocalDatabases(List<ExistingDatabase> databasesToDelete)
    {
        foreach (var database in databasesToDelete.Where(database => !string.IsNullOrEmpty(database.FilePath) && File.Exists(database.FilePath)))
        {
            File.Delete(database.FilePath);

            var backupDirectory = Path.Join(DbContextBackup.LocalDirectoryBackupDatabase, database.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory))
            {
                Directory.Delete(backupDirectory, true);
            }
        }

        RefreshExistingDatabases();
    }

    private async Task DisplaySuccessMessage()
    {
        await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseSuccessTitle,
            MainPageResources.MessageBoxRemoveDataBaseSuccessMessage,
            MainPageResources.MessageBoxRemoveDataBaseSuccessOkButton);
    }

    private async Task ExportToCloudAsync(List<ExistingDatabase> existingDatabasesSelected)
    {
        this.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorExportDatabaseToCloud);
        Log.Information("Starting to export database to cloud storage");

        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
            _ = await dropboxService.UploadFileAsync(existingDatabase.FilePath, DbContextBackup.CloudDirectoryBackupDatabase);
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

        this.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorExportDatabaseToLocalDatabase);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            var newFilePath = Path.Join(selectedFolder, existingDatabase.FileName);
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
            await Task.Run(() => File.Copy(existingDatabase.FilePath, newFilePath, true));
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
        }
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
    }

    [SupportedOSPlatform("Android")]
    [SupportedOSPlatform("iOS14.0")]
    [SupportedOSPlatform("MacCatalyst14.0")]
    [SupportedOSPlatform("Windows")]
    private async Task ExportToLocalFolderAsync(List<ExistingDatabase> existingDatabasesSelected, bool isCompress)
    {
        var folderPickerResult = await FolderPicker.Default.PickAsync();
        if (!folderPickerResult.IsSuccessful) return;

        var selectedFolder = folderPickerResult.Folder.Path;

        Log.Information("Starting to export database to {SelectedDialog}", selectedFolder);

        this.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorExportDatabaseToLocal);
        var failedExistingDatabases = new List<ExistingDatabase>();
        await Task.Run(async () =>
        {
            foreach (var existingDatabase in existingDatabasesSelected)
            {
                Log.Information("Starting to export {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
                var success = await existingDatabase.ToFolderAsync(selectedFolder, isCompress);
                if (!success) failedExistingDatabases.Add(existingDatabase);
                else Log.Information("Successfully exported {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
            }
        });
        CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();

        if (failedExistingDatabases.Count > 0) throw new Exception("Failed to export some databases");
        Log.Information("Database successfully copied to local storage");
    }

    private async Task HandleButtonAddDataBase()
    {
        var addDatabaseFileContentPage = new AddDatabaseFileContentPage();
        addDatabaseFileContentPage.SetExistingDatabase(ExistingDatabases);

        await Navigation.PushAsync(addDatabaseFileContentPage);

        var result = await addDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        this.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorCreateNewDatabase);

        var fileName = addDatabaseFileContentPage.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DbContextBackup.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DbContextBackup.LocalFilePathDataBaseModel, filePath, true);

            await using var context = new DataBaseContext(filePath);
            _ = context.SetAllDefaultValues();
            await context.SaveChangesAsync();

            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Information("New database was successfully added");
            await DisplayAlert(MainPageResources.MessageBoxAddDataBaseSuccessTitle, MainPageResources.MessageBoxAddDataBaseSuccessMessage, MainPageResources.MessageBoxAddDataBaseOkButton);
        }
        catch (Exception exception)
        {
            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
            Log.Error(exception, "An error occur");

            await DisplayAlert(MainPageResources.MessageBoxAddDataBaseErrorTitle, MainPageResources.MessageBoxAddDataBaseErrorMessage, MainPageResources.MessageBoxAddDataBaseOkButton);
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

        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);
        await Navigation.PushAsync(selectDatabaseFileContentPage);
        var result = await selectDatabaseFileContentPage.ResultDialog;
        if (result is not true) return;
        if (selectDatabaseFileContentPage.ExistingDatabasesSelected.Count.Equals(0)) return;

        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Database:
                    await ExportToLocalDatabase(selectDatabaseFileContentPage.ExistingDatabasesSelected);
                    break;

                case SaveLocation.Folder:
                    await ExportToLocalFolderAsync(selectDatabaseFileContentPage.ExistingDatabasesSelected, false);
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

            await DisplayAlert(MainPageResources.MessageBoxExportDataBaseSuccessTitle, MainPageResources.MessageBoxExportDataBaseSuccessMessage, MainPageResources.MessageBoxExportDataBaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await DisplayAlert(MainPageResources.MessageBoxExportDataBaseErrorTitle, MainPageResources.MessageBoxExportDataBaseErrorMessage, MainPageResources.MessageBoxExportDataBaseErrorOkButton);
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
            await DisplayAlert(MainPageResources.MessageBoxImportDatabaseSuccessTitle, MainPageResources.MessageBoxImportDatabaseSuccessMessage, MainPageResources.MessageBoxImportDatabaseSuccessOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await DisplayAlert(MainPageResources.MessageBoxImportDatabaseErrorTitle, MainPageResources.MessageBoxImportDatabaseErrorMessage, MainPageResources.MessageBoxImportDatabaseErrorOkButton);
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
        var metadatas = await dropboxService.ListFileAsync(DbContextBackup.CloudDirectoryBackupDatabase);
        metadatas = metadatas.Where(s => Path.GetExtension(s.PathDisplay).Equals(DbContextBackup.Extension));

        var existingDatabase = metadatas.Select(s => new ExistingDatabase(s.PathDisplay));

        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(existingDatabase);

        await Navigation.PushAsync(selectDatabaseFileContentPage);

        var result = await selectDatabaseFileContentPage.ResultDialog;

        if (result is not true)
        {
            Log.Warning("Import cancelled. No database selected");
            return;
        }

        this.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorImportDatabaseFromCloud);
        var mauiClient = HttpClientHandlerCustom.CreateHttpClientHandler();
        var files = selectDatabaseFileContentPage.ExistingDatabasesSelected.Select(s => s.FilePath);
        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

            var fileTemp = Path.Join(AppContext.BaseDirectory, "temp.sqlite");

            var temp = await dropboxService.DownloadFileAsync(file, fileTemp, mauiClient);
            Log.Information("Downloading {FileName} from cloud storage", fileName);
            File.Move(temp, newFilePath, true);
            Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
        }
    }

    private static async Task ImportFromLocalAsync(MainPage mainPage)
        {
            Log.Information("Starting to import the database from local storage");
            var dictionary = new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, ["public.database"] },
                { DevicePlatform.Android, ["application/octet-stream"] },
                { DevicePlatform.MacCatalyst, ["public.database"] }
            };

            var filePickerOption = new PickOptions { FileTypes = new FilePickerFileType(dictionary) };

            var result = await FilePicker.PickAsync(filePickerOption);
            if (result is null) return;

            mainPage.ShowCustomPopupActivityIndicator(MainPageResources.CustomPopupActivityIndicatorImportDatabaseFromLocal);
            var filePath = result.FullPath;

            var fileName = Path.GetFileName(filePath);
            var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

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
    }

    private async Task<List<ExistingDatabase>?> SelectDatabases()
    {
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);
        await Navigation.PushAsync(selectDatabaseFileContentPage);

        var result = await selectDatabaseFileContentPage.ResultDialog;
        return result ? selectDatabaseFileContentPage.ExistingDatabasesSelected : null;
    }

    #endregion
}