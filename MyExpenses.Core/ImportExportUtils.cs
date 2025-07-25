﻿using System.Collections.ObjectModel;
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
    /// Returns a collection of existing databases from cloud storage.
    /// </summary>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, for handling specific operations.</param>
    /// <returns>A collection of existing databases from cloud storage.</returns>
    public static async Task<IEnumerable<ExistingDatabase>> GetExistingCloudDatabase(ProjectSystem projectSystem)
    {
        var dropboxService = await DropboxService.CreateAsync(projectSystem);

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
            existingDatabase.SyncStatus = await localDatabase.CheckStatus(projectSystem);
        }

        return existingDatabases;
    }

    /// <summary>
    /// Downloads the selected databases from cloud storage.
    /// </summary>
    /// <param name="files">The file paths of the databases to download.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, for handling specific operations.</param>
    /// <param name="client">An optional HttpClient that can be used to handle HTTP requests and responses.</param>
    /// <returns></returns>
    public static async Task DownloadDropboxFiles(IEnumerable<string> files, ProjectSystem projectSystem,
        HttpClient? client = null)
    {
        var dropboxService = await DropboxService.CreateAsync(projectSystem);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var newFilePath = Path.Join(DatabaseInfos.LocalDirectoryDatabase, fileName);

            var fileTemp = Path.Join(AppContext.BaseDirectory, "temp.sqlite");

            Log.Information("Downloading {FileName} from cloud storage", fileName);
            var temp = await dropboxService.DownloadFileAsync(file, fileTemp, client);

            File.Move(temp, newFilePath, true);
            Log.Information("Successfully downloaded {FileName} from cloud storage", fileName);
        }
    }

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

    /// <summary>
    /// Executes the vacuum operation on all existing databases retrieved from the database context, optimizing their storage
    /// by reclaiming unused space and reducing file size. Returns a list containing information about the size changes for each processed database.
    /// </summary>
    /// <returns>A list of <see cref="SizeDatabase"/> objects, each representing the storage details of a database before and after the vacuum operation.
    /// If an error occurs for a specific database, the list may contain null entries.</returns>
    public static List<SizeDatabase?> VacuumDatabases()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var sizeDatabases = new List<SizeDatabase?>();

        foreach (var existingDatabase in DbContextBackup.GetExistingDatabase())
        {
            var sizeDatabase = existingDatabase.VacuumDatabase();
            sizeDatabases.Add(sizeDatabase);
        }

        return sizeDatabases;
    }

    /// <summary>
    /// Performs a vacuum operation on the specified database file to optimize its size and structure.
    /// Returns the details of the size changes before and after the operation, or null if the operation fails.
    /// </summary>
    /// <param name="existingDatabase">The database to be vacuumed. Contains details such as file path and metadata.</param>
    /// <returns>
    /// A <see cref="SizeDatabase"/> object containing the size details of the database before and after the vacuum operation.
    /// Returns null if the vacuum operation fails.
    /// </returns>
    public static SizeDatabase? VacuumDatabase(this ExistingDatabase existingDatabase)
    {
        var oldSize = existingDatabase.FileInfo.Length;
        var result = existingDatabase.FilePath.VacuumDatabase();

        if (result is not true)
        {
            Log.Error("Error while vacuum database {FileName}", existingDatabase.FileName);
            return null;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var newSize = new FileInfo(existingDatabase.FilePath).Length;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var sizeDatabase = new SizeDatabase { FileNameWithoutExtension = existingDatabase.FileNameWithoutExtension };
        sizeDatabase.SetOldSize(oldSize);
        sizeDatabase.SetNewSize(newSize);

        return sizeDatabase;
    }
}