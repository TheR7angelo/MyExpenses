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

    private async void ButtonExportDataBase_OnClick(object sender, RoutedEventArgs e)
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

                default:
                    throw new ArgumentOutOfRangeException();
            }

            waitScreenWindow.Close();

            MsgBox.Show(WelcomePageResources.ButtonExportDataBaseSucess, MsgBoxImage.Check);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "An error occurred. Please try again");
            waitScreenWindow.Close();

            MsgBox.Show(WelcomePageResources.ButtonExportDataBaseError, MsgBoxImage.Warning);
        }
    }

    private async void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
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
                    await ImportFromLocal();
                    break;

                case SaveLocation.Dropbox:
                    waitScreenWindow.WaitMessage = WelcomePageResources.ButtonImportDataBaseWaitMessageImportFromCloud;
                    waitScreenWindow.Show();
                    await ImportFromCloudAsync();
                    break;

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

    private async void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
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

            var backupDirectory = Path.Join(DbContextBackup.LocalDirectoryBackupDatabase, existingDatabase.FileNameWithoutExtension);
            if (Directory.Exists(backupDirectory)) Directory.Delete(backupDirectory, true);
        }

        RefreshExistingDatabases();

        response = MsgBox.Show(WelcomePageResources.MessageBoxDeleteCloudQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNoCancel);

        if (response is MessageBoxResult.Yes)
        {
            var files = selectDatabaseFileWindow.ExistingDatabasesSelected.Select(s => s.FileName).ToArray();
            Log.Information("Preparing to delete the following files: {Files}", files);

            var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
            await dropboxService.DeleteFilesAsync(files, DbContextBackup.CloudDirectoryBackupDatabase);
        }

        Log.Information("Files successfully deleted");
        MsgBox.Show(WelcomePageResources.MessageBoxDeleteCloudQuestionSuccess, MsgBoxImage.Check, MessageBoxButton.OK);
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
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
            await dropboxService.UploadFileAsync(existingDatabase.FilePath, DbContextBackup.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
        }
    }

    private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    {
        var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
        Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
        await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath, DbContextBackup.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
    }

    private static async Task ExportToLocalFolderAsync(List<ExistingDatabase> existingDatabasesSelected, bool isCompress)
    {
        var folderDialog = new FolderDialog();
        var selectedDialog = folderDialog.GetFile();

        if (string.IsNullOrEmpty(selectedDialog))
        {
            Log.Warning("Export cancelled. No file path provided");
            return;
        }

        Log.Information("Starting to export database to {SelectedDialog}", selectedDialog);

        await Task.Run(async () =>
        {
            foreach (var existingDatabase in existingDatabasesSelected)
            {
                Log.Information("Starting to export {ExistingDatabaseFileName}", existingDatabase.FileNameWithoutExtension);
                await existingDatabase.ToFolderAsync(selectedDialog, isCompress);
            }
        });

        Log.Information("Database successfully copied to local storage");

        var response = MsgBox.Show(WelcomePageResources.MessageBoxOpenExportFolderQuestion, MsgBoxImage.Question,
            MessageBoxButton.YesNo);
        if (response is MessageBoxResult.Yes) selectedDialog.StartFile();
    }

    private static async Task SaveToLocalDatabase(List<ExistingDatabase> existingDatabasesSelected)
    {
        if (existingDatabasesSelected.Count is 1) await ExportToLocalDatabaseFileAsync(existingDatabasesSelected.First());
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
            Log.Information("Starting to copy {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
            await Task.Run(() => File.Copy(existingDatabase.FilePath, newFilePath, true));
            Log.Information("Successfully copied {ExistingDatabaseFileName} to {NewFilePath}", existingDatabase.FileName, newFilePath);
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
        await Task.Run(() =>
        {
            File.Copy(selectedFilePath, selectedDialog, true);
        });
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

    var existingDatabase = metadatas.Select(s => new ExistingDatabase(s.PathDisplay));
    var selectDatabaseFileWindow = new SelectDatabaseFileWindow();
    selectDatabaseFileWindow.ExistingDatabases.AddRange(existingDatabase);
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
    }

    #endregion
}