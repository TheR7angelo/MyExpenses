using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;
using Serilog;

namespace MyExpenses.Core;

public static class ImportExportUtils
{
    /// <summary>
    /// Refreshes the collection of existing databases by removing non-existent database files
    /// and adding or updating databases based on the current state of the file system.
    /// </summary>
    /// <param name="existingDatabases">The collection of existing databases to be refreshed.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, for handling specific operations.</param>
    public static void RefreshExistingDatabases(this ObservableCollection<ExistingDatabase> existingDatabases,
        ProjectSystem projectSystem)
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

        _ = existingDatabases.CheckExistingDatabaseIsSyncAsync(projectSystem);
    }

    #region Delete

    /// <summary>
    /// Deletes the specified cloud files associated with the provided databases in the target project system.
    /// </summary>
    /// <param name="databasesToDelete">A collection of databases whose associated cloud files should be deleted.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, to manage cloud file deletion.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public static async Task DeleteCloudFilesAsync(this IEnumerable<ExistingDatabase> databasesToDelete,
        ProjectSystem projectSystem)
    {
        var files = databasesToDelete.Select(db => db.FileName).ToArray();
        Log.Information("Preparing to delete the following files: {Files}", files);

        var dropboxService = await DropboxService.CreateAsync(projectSystem);
        _ = await dropboxService.DeleteFilesAsync(files, DatabaseInfos.CloudDirectoryBackupDatabase);

        Log.Information("Files successfully deleted from Dropbox");
    }

    /// <summary>
    /// Deletes the specified local databases and their corresponding backup directories
    /// from the local file system if they exist.
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
    /// Exports the selected database to a cloud storage location.
    /// </summary>
    /// <param name="existingDatabasesSelected">The database to be exported to the cloud.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, used for interacting with the cloud storage API.</param>
    /// <returns>A task representing the asynchronous operation of exporting the database to the cloud.</returns>
    public static async Task ExportToCloudFileAsync(this ExistingDatabase existingDatabasesSelected,
        ProjectSystem projectSystem)
    {
        var dropboxService = await DropboxService.CreateAsync(projectSystem);
        Log.Information("Starting to upload {FileName} to cloud storage", existingDatabasesSelected.FileName);
        _ = await dropboxService.UploadFileAsync(existingDatabasesSelected.FilePath,
            DatabaseInfos.CloudDirectoryBackupDatabase);
        Log.Information("Successfully uploaded {FileName} to cloud storage", existingDatabasesSelected.FileName);
    }

    public static async Task ExportToCloudFileAsync(this List<ExistingDatabase> existingDatabasesSelected, ProjectSystem projectSystem)
    {
        Log.Information("Starting to export database to cloud storage");

        var dropboxService = await DropboxService.CreateAsync(projectSystem);
        foreach (var existingDatabase in existingDatabasesSelected)
        {
            Log.Information("Starting to upload {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
            _ = await dropboxService.UploadFileAsync(existingDatabase.FilePath, DatabaseInfos.CloudDirectoryBackupDatabase);
            Log.Information("Successfully uploaded {ExistingDatabaseFileName} to cloud storage", existingDatabase.FileName);
        }

        Log.Information("Finished exporting all selected databases to cloud storage");

    }

    /// <summary>
    /// Saves the selected databases to a cloud storage location, handling single or multiple databases appropriately.
    /// </summary>
    /// <param name="existingDatabasesSelected">The list of databases to be saved to the cloud.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, used for interacting with the cloud storage API.</param>
    /// <returns>A task representing the asynchronous operation of saving the databases to the cloud.</returns>
    public static async Task SaveToCloudAsync(this List<ExistingDatabase> existingDatabasesSelected,
        ProjectSystem projectSystem)
    {
        if (existingDatabasesSelected.Count is 1) await existingDatabasesSelected.First().ExportToCloudFileAsync(projectSystem);
        else await existingDatabasesSelected.ExportToCloudFileAsync(projectSystem);

        await existingDatabasesSelected.CheckExistingDatabaseIsSyncAsync(projectSystem);
    }

    #endregion
}