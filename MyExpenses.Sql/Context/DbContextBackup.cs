using MyExpenses.Models.IO;
using MyExpenses.SharedUtils.GlobalInfos;

namespace MyExpenses.Sql.Context;

public static class DbContextBackup
{
    /// <summary>
    /// Retrieves all existing database files from the local database directory.
    /// Searches for database files with the specified extension within the top directory only.
    /// </summary>
    /// <returns>An array of ExistingDatabase objects representing the database files found.</returns>
    public static ExistingDatabase[] GetExistingDatabase()
    {
        var existingDatabases = Directory
            .GetFiles(DatabaseInfos.LocalDirectoryDatabase, $"*{DatabaseInfos.Extension}", SearchOption.TopDirectoryOnly)
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Using an array instead of List<T> because the number of files is very small (less than 100, typically around 10)
            // and the collection is fixed after being loaded, so no resizing or modifications are needed.
            .Select(s => new ExistingDatabase(s)).ToArray();

        return existingDatabases;
    }

    /// <summary>
    /// Retrieves all existing backup directories within the local backup database directory.
    /// Ensures the backup directory is created if it doesn't already exist.
    /// </summary>
    /// <returns>An array of strings representing the paths to the backup directories found.</returns>
    public static string[] GetExistingBackupDirectoryDatabase()
    {
        if (!Directory.Exists(DatabaseInfos.LocalDirectoryBackupDatabase)) Directory.CreateDirectory(DatabaseInfos.LocalDirectoryBackupDatabase);

        return Directory.GetDirectories(DatabaseInfos.LocalDirectoryBackupDatabase);
    }

    /// <summary>
    /// Retrieves all existing backup database files from the specified local backup database directory.
    /// Ensures only files with the specified extension are included in the search.
    /// </summary>
    /// <param name="localDirectoryBackupDatabase">The path to the local backup database directory to search within.</param>
    /// <returns>An array of ExistingDatabase objects representing the backup database files found.</returns>
    public static ExistingDatabase[] GetExistingBackupDatabase(this string localDirectoryBackupDatabase)
    {
        if (!Directory.Exists(localDirectoryBackupDatabase)) return [];

        var existingDatabases = Directory
            .GetFiles(localDirectoryBackupDatabase, $"*{DatabaseInfos.Extension}", SearchOption.TopDirectoryOnly)
            .Select(s => new ExistingDatabase(s)).ToArray();

        return existingDatabases;
    }

    /// <summary>
    /// Cleans up backup files in the local database backup directory by ensuring the total number of backups per database does not exceed the specified maximum.
    /// Older backup files are deleted until the limit is met.
    /// </summary>
    /// <param name="maxDatabaseBackup">The maximum allowed backups per database in the local backup directory.</param>
    /// <returns>The total number of backup files deleted.</returns>
    public static int CleanBackupDatabase(int maxDatabaseBackup)
    {
        var totalDelete = 0;

        if (!Directory.Exists(DatabaseInfos.LocalDirectoryBackupDatabase)) return totalDelete;

        var directories = Directory.GetDirectories(DatabaseInfos.LocalDirectoryBackupDatabase);
        foreach (var directory in directories)
        {
            var files = Directory.GetFiles(directory, $"*{DatabaseInfos.Extension}").ToList();
            if (files.Count <= maxDatabaseBackup) continue;

            files = files.OrderBy(s => new FileInfo(s).CreationTime).ToList();
            while (files.Count >= maxDatabaseBackup)
            {
                File.Delete(files[0]);
                files.RemoveAt(0);
                Interlocked.Increment(ref totalDelete);
            }
        }

        return totalDelete;
    }

    /// <summary>
    /// Backs up existing databases to a defined local directory.
    /// Creates a backup directory for each database and saves the files with a timestamp.
    /// Only databases with valid file paths and existing files will be backed up.
    /// </summary>
    /// <returns>The total number of databases successfully backed up.</returns>
    public static int BackupDatabase()
    {
        var existingDatabases = GetExistingDatabase();

        var totalBackup = 0;
        if (existingDatabases.Length == 0) return totalBackup;

        if (!Directory.Exists(DatabaseInfos.LocalDirectoryBackupDatabase)) Directory.CreateDirectory(DatabaseInfos.LocalDirectoryBackupDatabase);

        foreach (var existingDatabase in existingDatabases)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            var directory = Path.Join(DatabaseInfos.LocalDirectoryBackupDatabase, existingDatabase.FileNameWithoutExtension);
            Directory.CreateDirectory(directory);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var extension = Path.GetExtension(existingDatabase.FilePath);
            var destinationFileName = Path.Join(directory, $"{existingDatabase.FileNameWithoutExtension}_{timestamp}{extension}");
            File.Copy(existingDatabase.FilePath, destinationFileName, true);

            Interlocked.Increment(ref totalBackup);
        }

        return totalBackup;
    }
}