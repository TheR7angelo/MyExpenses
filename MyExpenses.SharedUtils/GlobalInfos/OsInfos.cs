namespace MyExpenses.SharedUtils.GlobalInfos;

public static class OsInfos
{
    public static string OsBasePath => AppContext.BaseDirectory;

    public static string LogDirectoryPath { get; }

    public static string ConfigurationFilePath { get; } = Path.Join(OsBasePath, "appsettings.json");
    public static string AppVersionInfo { get; } = Path.Join(OsBasePath, "AppVersionInfo.json");

    static OsInfos()
    {
        LogDirectoryPath = Path.Join(OsBasePath, "log");
        if (!Directory.Exists(LogDirectoryPath)) Directory.CreateDirectory(LogDirectoryPath);
    }
}