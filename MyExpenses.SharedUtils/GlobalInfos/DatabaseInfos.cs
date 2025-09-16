namespace MyExpenses.SharedUtils.GlobalInfos;

public static class DatabaseInfos
{
    public static string LocalDirectoryDatabase { get; }
    public static string LocalDirectoryBackupDatabase { get; }

    public static string CloudDirectoryBackupDatabase => "Databases";

    public static string Extension => ".sqlite";

    public static string SearchPatternExtension => $"*{Extension}";

    public static string LocalFilePathDataBaseModel { get; }

    public static string FormatDateTimeBackup => "yyyyMMdd_HHmmss";

    static DatabaseInfos()
    {
        LocalDirectoryDatabase = Path.Join(OsInfos.OsBasePath, "Databases");
        if (!Directory.Exists(LocalDirectoryDatabase)) Directory.CreateDirectory(LocalDirectoryDatabase);

        LocalDirectoryBackupDatabase = Path.Join(LocalDirectoryDatabase, "Backups");
        if (!Directory.Exists(LocalDirectoryBackupDatabase)) Directory.CreateDirectory(LocalDirectoryBackupDatabase);

        LocalFilePathDataBaseModel = Path.Join(OsInfos.OsBasePath, "Database Models", "Model.sqlite");
    }
}