using System.IO;
using System.Windows;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Resources.Resx.WelcomeManagement;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Wpf.Utils.FilePicker;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf;

public static class ImportExportUtils
{
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
}