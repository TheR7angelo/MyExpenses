using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Core;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.Models.Wpf.Save;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
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
        ExistingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

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
        => _ = ExistingDatabases.HandleButtonExportDataBase();

    private void ButtonImportDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonImportDataBase();

    private void ButtonRemoveDataBase_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonRemoveDataBase();

    #endregion

    #region Function

    private static bool ConfirmDeletion(string message)
    {
        var response = MsgBox.Show(message, MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        return response is MessageBoxResult.Yes;
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

    // private static async Task ExportToCloudFileAsync(ExistingDatabase existingDatabasesSelected)
    // {
    //     var dropboxService = await DropboxService.CreateAsync(ProjectSystem.Wpf);
    //     Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
    //     _ = await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath,
    //         DatabaseInfos.CloudDirectoryBackupDatabase);
    //     Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
    // }

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

            ExistingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

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
        ExistingDatabases.RefreshExistingDatabases(ProjectSystem.Wpf);

        var confirmCloudDeletion = ConfirmDeletion(WelcomeManagementResources.MessageBoxRemoveDataBaseDropboxQuestionMessage);
        if (!confirmCloudDeletion) return;

        await DeleteCloudFilesAsync(selectedDatabases);

        MsgBox.Show(WelcomeManagementResources.MessageBoxRemoveDataBaseSuccessMessage, MsgBoxImage.Check, MessageBoxButton.OK);
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

    #endregion
}