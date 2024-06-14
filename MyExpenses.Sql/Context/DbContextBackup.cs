using MyExpenses.Models.IO;

namespace MyExpenses.Sql.Context;

public static class DbContextBackup
{
    public static string DirectoryDatabase { get; } = Path.GetFullPath("Databases");
    public static string DirectoryBackupDatabase { get; } = Path.Join(DirectoryDatabase, "Backups");

    public static string Extension => ".sqlite";

    public static ExistingDatabase[] GetExistingDatabase()
    {
        Directory.CreateDirectory(DirectoryDatabase);

        var existingDatabases = Directory
            .GetFiles(DirectoryDatabase, $"*{Extension}", SearchOption.TopDirectoryOnly)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static ExistingDatabase[] GetExistingBackupDatabase()
    {
        Directory.CreateDirectory(DirectoryBackupDatabase);

        var existingDatabases = Directory
            .GetFiles(DirectoryBackupDatabase, $"*{Extension}", SearchOption.AllDirectories)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static int CleanBackupDatabase()
    {
        const byte maxDatabaseBackup = 15;
        var totalDelete = 0;

        if (!Directory.Exists(DirectoryBackupDatabase)) return totalDelete;

        var directories = Directory.GetDirectories(DirectoryBackupDatabase);
        foreach (var directory in directories)
        {
            var files = Directory.GetFiles(directory, $"*{Extension}").ToList();
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

    public static int BackupDatabase()
    {
        var existingDatabases = GetExistingDatabase();

        var totalBackup = 0;
        if (existingDatabases.Length == 0) return totalBackup;

        var directoryBackup = DirectoryBackupDatabase;
        Directory.CreateDirectory(directoryBackup);

        foreach (var existingDatabase in existingDatabases)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            var directory = Path.Join(directoryBackup, existingDatabase.FileNameWithoutExtension);
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