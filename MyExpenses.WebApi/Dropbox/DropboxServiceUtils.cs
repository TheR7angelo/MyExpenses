namespace MyExpenses.WebApi.Dropbox;

public static class DropboxServiceUtils
{
    /// <summary>
    /// Gets the directory path designated for storing the secret keys used in Dropbox integration.
    /// This directory typically includes files containing sensitive information, such as authentication tokens,
    /// and has specific attributes set to enhance security and ensure proper organization.
    /// </summary>
    private static string DirectorySecretKeys { get; } = GenerateDirectorySecretKeys();

    /// <summary>
    /// Gets the file path for storing the secret keys used in Dropbox integration.
    /// The file typically contains authentication and token information for secure access.
    /// This property combines the directory path for secret keys with a predefined file name,
    /// such as "AccessTokenAuthentication.json".
    /// </summary>
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