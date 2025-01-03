namespace MyExpenses.WebApi.Dropbox;

public static class DropboxServiceUtils
{
    private static string DirectorySecretKeys { get; } = GenerateDirectorySecretKeys();
    public static string FilePathSecretKeys { get; } = Path.Join(DirectorySecretKeys, "AccessTokenAuthentication.json");

    private static string GenerateDirectorySecretKeys()
    {
        var directorySecretKeys = Path.Join(AppContext.BaseDirectory, "Api", "Dropbox");

        var directoryInfo = Directory.CreateDirectory(directorySecretKeys);
        directoryInfo = directoryInfo.Parent;
        if (directoryInfo is not null) directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        return directorySecretKeys;
    }

    public static bool IsDropboxEnabled()
        => File.Exists(FilePathSecretKeys);
}