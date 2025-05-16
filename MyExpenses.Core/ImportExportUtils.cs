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
}