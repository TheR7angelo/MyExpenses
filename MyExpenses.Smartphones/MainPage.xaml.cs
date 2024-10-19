using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Smartphones.AppShells;
using MyExpenses.Smartphones.ContentPages;
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

    #region Function

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

    #endregion

    #region Action

    private async void ButtonAddDataBase_OnClick(object? sender, EventArgs e)
    {
        var addDatabaseFileContentPage = new AddDatabaseFileContentPage();
        addDatabaseFileContentPage.SetExistingDatabase(ExistingDatabases);

        await Navigation.PushAsync(addDatabaseFileContentPage);

        var result = await addDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        var fileName = addDatabaseFileContentPage.DatabaseFilename;
        fileName = Path.ChangeExtension(fileName, ".sqlite");
        var filePath = Path.Combine(DbContextBackup.LocalDirectoryDatabase, fileName);

        Log.Information("Create new database with name \"{FileName}\"", fileName);

        try
        {
            File.Copy(DbContextBackup.LocalFilePathDataBaseModel, filePath, true);

            await using var context = new DataBaseContext(filePath);
            context.SetAllDefaultValues();
            await context.SaveChangesAsync();

            ExistingDatabases.AddAndSort(new ExistingDatabase(filePath),
                s => s.FileNameWithoutExtension);

            Log.Information("New database was successfully added");
            await DisplayAlert(MainPageResources.MessageBoxAddDataBaseSuccessTitle, MainPageResources.MessageBoxAddDataBaseSuccessMessage, MainPageResources.MessageBoxAddDataBaseOkButton);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occur");

            await DisplayAlert(MainPageResources.MessageBoxAddDataBaseErrorTitle, MainPageResources.MessageBoxAddDataBaseErrorMessage, MainPageResources.MessageBoxAddDataBaseOkButton);
        }
    }

    private void ButtonDatabase_OnClick(object? sender, EventArgs e)
    {
        var buttonImageView = (ButtonImageTextView)sender!;
        if (buttonImageView.BindingContext is not ExistingDatabase existingDatabase) return;

        DataBaseContext.FilePath = existingDatabase.FilePath;

        var dashBoardShell = new DashBoardShell();
        Application.Current!.MainPage = dashBoardShell;
    }

    private async void ButtonRemoveDataBase_OnClick(object? sender, EventArgs e)
    {
        var selectDatabaseFileContentPage = new SelectDatabaseFileContentPage();
        selectDatabaseFileContentPage.ExistingDatabases.AddRange(ExistingDatabases);

        await Navigation.PushAsync(selectDatabaseFileContentPage);

        var result = await selectDatabaseFileContentPage.ResultDialog;

        if (result is not true) return;

        if (selectDatabaseFileContentPage.ExistingDatabasesSelected.Count.Equals(0)) return;

        var response = await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseQuestionTitle,
            MainPageResources.MessageBoxRemoveDataBaseQuestionMessage,
            MainPageResources.MessageBoxRemoveDataBaseQuestionYesButton,
            MainPageResources.MessageBoxRemoveDataBaseQuestionCancelButton);

        if (!response) return;

        foreach (var existingDatabase in selectDatabaseFileContentPage.ExistingDatabasesSelected)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            File.Delete(existingDatabase.FilePath);

            var backupDirectory = Path.Join(DbContextBackup.LocalDirectoryBackupDatabase, existingDatabase.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory)) Directory.Delete(backupDirectory, true);
        }

        RefreshExistingDatabases();


        response = await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionTitle,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionMessage,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionYesButton,
            MainPageResources.MessageBoxRemoveDataBaseDropboxQuestionNoButton);

        if (response)
        {
            var files = selectDatabaseFileContentPage.ExistingDatabasesSelected.Select(s => s.FileName).ToArray();
            Log.Information("Preparing to delete the following files: {Files}", files);

            var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Maui);
            await dropboxService.DeleteFilesAsync(files, DbContextBackup.CloudDirectoryBackupDatabase);
        }

        Log.Information("Files successfully deleted");
        await DisplayAlert(MainPageResources.MessageBoxRemoveDataBaseSuccessTitle,
            MainPageResources.MessageBoxRemoveDataBaseSuccessMessage,
            MainPageResources.MessageBoxRemoveDataBaseSuccessOkButton);
    }

    #endregion

    private async void ButtonImportDataBase_OnClick(object? sender, EventArgs e)
    {
        var saveLocationContentPage = new SaveLocationContentPage();

        await Navigation.PushAsync(saveLocationContentPage);

        var result = await saveLocationContentPage.ResultDialog;

        if (result is not true) return;
        
        var saveLocation = saveLocationContentPage.SaveLocationResult;
        
        try
        {
            switch (saveLocation)
            {
                case SaveLocation.Local:
                    await ImportFromLocalAsync();
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

            await DisplayAlert("Title", "Database import operation was successful", "Ok");
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            await DisplayAlert("Title", "An error occurred. Please try again", "Ok");
        }
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

        var files = selectDatabaseFileContentPage.ExistingDatabasesSelected.Select(s => s.FilePath);
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

    private static async Task ImportFromLocalAsync()
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
        
        var filePath = result.FullPath;
        
        var fileName = Path.GetFileName(filePath);
        var newFilePath = Path.Join(DbContextBackup.LocalDirectoryDatabase, fileName);

        Log.Information("Copying {FileName} to local storage", fileName);
        File.Copy(filePath, newFilePath, true);
        Log.Information("Successfully copied {FileName} to local storage", fileName);
    }

    private void ButtonExportDataBase_OnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}