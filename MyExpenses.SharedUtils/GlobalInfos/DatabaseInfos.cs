namespace MyExpenses.SharedUtils.GlobalInfos;

public static class DatabaseInfos
{
    public static string LocalDirectoryDatabase { get; }
    public static string LocalDirectoryBackupDatabase { get; }

    public static string CloudDirectoryBackupDatabase => "Databases";

    public static string Extension => ".sqlite";

    public static string LocalFilePathDataBaseModel { get; }

    static DatabaseInfos()
    {
        LocalDirectoryDatabase = Path.Join(OsInfos.OsBasePath, "Databases");
        if (!Directory.Exists(LocalDirectoryDatabase)) Directory.CreateDirectory(LocalDirectoryDatabase);

        LocalDirectoryBackupDatabase = Path.Join(LocalDirectoryDatabase, "Backups");
        LocalFilePathDataBaseModel = Path.Join(OsInfos.OsBasePath, "Database Models", "Model.sqlite");
    }
}