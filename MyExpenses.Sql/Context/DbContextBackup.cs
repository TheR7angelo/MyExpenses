using MyExpenses.Models.IO;

namespace MyExpenses.Sql.Context;

public static class DbContextBackup
{
    public static string LocalDirectoryDatabase { get; } = Path.GetFullPath("Databases");
    public static string LocalDirectoryBackupDatabase { get; } = Path.Join(LocalDirectoryDatabase, "Backups");

    public static string CloudDirectoryBackupDatabase => "Databases";

    public static string Extension => ".sqlite";

    public static ExistingDatabase[] GetExistingDatabase()
    {
        Directory.CreateDirectory(LocalDirectoryDatabase);

        var existingDatabases = Directory
            .GetFiles(LocalDirectoryDatabase, $"*{Extension}", SearchOption.TopDirectoryOnly)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static ExistingDatabase[] GetExistingBackupDatabase()
    {
        Directory.CreateDirectory(LocalDirectoryBackupDatabase);

        var existingDatabases = Directory
            .GetFiles(LocalDirectoryBackupDatabase, $"*{Extension}", SearchOption.AllDirectories)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static int CleanBackupDatabase(int maxDatabaseBackup)
    {
        var totalDelete = 0;

        if (!Directory.Exists(LocalDirectoryBackupDatabase)) return totalDelete;

        var directories = Directory.GetDirectories(LocalDirectoryBackupDatabase);
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

        var directoryBackup = LocalDirectoryBackupDatabase;
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