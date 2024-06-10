using MyExpenses.Models.IO;

namespace MyExpenses.Sql.Context;

public static class DbContextBackup
{
    public static string DirectoryDatabase { get; } = Path.GetFullPath("Databases");
    public static string DirectoryBackupDatabase { get; } = Path.Join(DirectoryDatabase, "Backups");

    public static ExistingDatabase[] GetExistingDatabase()
    {
        Directory.CreateDirectory(DirectoryDatabase);

        var existingDatabases = Directory
            .GetFiles(DirectoryDatabase, "*.sqlite", SearchOption.TopDirectoryOnly)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static ExistingDatabase[] GetExistingBackupDatabase()
    {
        Directory.CreateDirectory(DirectoryBackupDatabase);

        var existingDatabases = Directory
            .GetFiles(DirectoryBackupDatabase, "*.sqlite", SearchOption.AllDirectories)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static int CleanBackupDatabase()
    {
        var existingDatabases = GetExistingBackupDatabase();

        var totalDelete = 0;
        if (existingDatabases.Length == 0) return totalDelete;

        var now = DateTime.Now.AddDays(-15);
        foreach (var existingDatabase in existingDatabases)
        {
            if (string.IsNullOrEmpty(existingDatabase.FilePath)) continue;
            if (!File.Exists(existingDatabase.FilePath)) continue;

            var fileInfo = new FileInfo(existingDatabase.FilePath);
            if (fileInfo.LastWriteTime >= now) continue;

            fileInfo.Delete();
            Interlocked.Increment(ref totalDelete);
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