namespace MyExpenses.WebApi.Dropbox;

public static class DropboxServiceUtils
{
    private static string DirectorySecretKeys { get; } = GenerateDirectorySecretKeys();
    public static string FilePathSecretKeys { get; } = Path.Join(DirectorySecretKeys, "AccessTokenAuthentication.json");

    /// <summary>
    /// Creates and returns the directory path designated for storing Dropbox secret keys.
    /// This includes ensuring the directory exists and setting its attributes to hidden.
    /// </summary>
    /// <returns>A string representing the path to the directory for Dropbox secret keys.</returns>
    private static string GenerateDirectorySecretKeys()
    {
        var directorySecretKeys = Path.Join(AppContext.BaseDirectory, "Api", "Dropbox");

        var directoryInfo = Directory.CreateDirectory(directorySecretKeys);
        directoryInfo = directoryInfo.Parent;
        if (directoryInfo is not null) directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        return directorySecretKeys;
    }

    /// <summary>
    /// Determines whether the Dropbox integration is enabled by checking the existence of the secret keys file.
    /// </summary>
    /// <returns>A boolean value indicating whether the Dropbox integration is enabled.</returns>
    public static bool IsDropboxEnabled()
        => File.Exists(FilePathSecretKeys);
}