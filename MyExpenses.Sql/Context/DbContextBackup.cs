using MyExpenses.Models.IO;

namespace MyExpenses.Sql.Context;

public static class DbContextBackup
{
    public static ExistingDatabase[] GetExistingDatabase()
    {
        var directoryDatabase = Path.GetFullPath("Databases");
        Directory.CreateDirectory(directoryDatabase);

        var existingDatabases = Directory
            .GetFiles(directoryDatabase, "*.sqlite", SearchOption.TopDirectoryOnly)
            .Select(s => new ExistingDatabase { FilePath = s } ).ToArray();

        return existingDatabases;
    }

    public static void BackupDatabase()
    {
        var existingDatabases = GetExistingDatabase();

        if (existingDatabases.Length == 0) return;

        var directoryBackup = Path.GetDirectoryName(existingDatabases.First().FilePath);
        directoryBackup = Path.Join(directoryBackup, "Backups");
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
        }
    }
}