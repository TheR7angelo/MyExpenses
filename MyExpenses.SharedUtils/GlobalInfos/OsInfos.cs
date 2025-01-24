namespace MyExpenses.SharedUtils.GlobalInfos;

public static class OsInfos
{
    public static string OsBasePath { get; }

    public static string LogDirectoryPath { get; }

    public static string ConfigurationFilePath { get; } = Path.Join(OsBasePath, "appsettings.json");
    public static string AppVersionInfo { get; } = Path.Join(OsBasePath, "AppVersionInfo.json");

    static OsInfos()
    {
        OsBasePath = AppContext.BaseDirectory;

        LogDirectoryPath = Path.Join(OsBasePath, "log");
        if (!Directory.Exists(LogDirectoryPath)) Directory.CreateDirectory(LogDirectoryPath);
    }
}