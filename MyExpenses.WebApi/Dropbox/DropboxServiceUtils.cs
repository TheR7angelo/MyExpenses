using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.Models.WebApi.DropBox;
using MyExpenses.SharedUtils.GlobalInfos;

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
    /// This property combines the directory path for secret keys with a predefined filename,
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
    private static bool IsDropboxEnabled()
        => File.Exists(FilePathSecretKeys);

    /// <summary>
    /// Checks the synchronization status of the given existing database against a connected Dropbox service.
    /// Determines whether the database is unmatched, synchronized, or if one copy (local or remote) is outdated.
    /// </summary>
    /// <param name="existingDatabase">The existing database to evaluate for synchronization status.</param>
    /// <param name="projectSystem">The project system context (for example, Wpf or Maui) used to create and initialize a Dropbox service instance.</param>
    /// <returns>A task representing the asynchronous operation, containing the synchronization status of the database as a <see cref="SyncStatus"/> value.</returns>
    // ReSharper disable once HeapView.ClosureAllocation
    public static async Task<SyncStatus> CheckStatus(this ExistingDatabase existingDatabase,
        ProjectSystem projectSystem)
    {
        if (!IsDropboxEnabled()) return SyncStatus.UnSynchronized;
        if (!File.Exists(existingDatabase.FilePath)) return SyncStatus.UnSynchronized;

        var dropboxService = await DropboxService.CreateAsync(projectSystem);
        var cloudDatabaseFiles = await dropboxService.ListFileAsync(DatabaseInfos.CloudDirectoryBackupDatabase);

        // ReSharper disable once HeapView.DelegateAllocation
        var cloudDatabase = cloudDatabaseFiles.FirstOrDefault(s => s.Name.Equals(existingDatabase.FileInfo.Name));
        if (cloudDatabase is null) return SyncStatus.UnSynchronized;

        var cloudDatabaseFile = cloudDatabase.AsFile;
        var cloudDatabaseHashContent = cloudDatabaseFile.ContentHash;
        var localDatabaseHashContent = existingDatabase.GetDropboxContentHash();

        if (cloudDatabaseHashContent.Equals(localDatabaseHashContent, StringComparison.Ordinal)) return SyncStatus.Synchronized;

        return cloudDatabase.AsFile.ClientModified.ToUniversalTime() > existingDatabase.FileInfo.LastWriteTimeUtc
            ? SyncStatus.LocalIsOutdated
            : SyncStatus.RemoteIsOutdated;
    }

    /// <summary>
    /// Checks the synchronization status of all specified existing databases in relation to the provided project system.
    /// Updates the synchronization status for each database accordingly.
    /// </summary>
    /// <param name="existingDatabases">A collection of existing databases to be checked for synchronization.</param>
    /// <param name="projectSystem">The project system context that determines the synchronization behavior.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task CheckExistingDatabaseIsSyncAsync(this IEnumerable<ExistingDatabase> existingDatabases,
        ProjectSystem projectSystem)
    {
        foreach (var existingDatabase in existingDatabases)
        {
            var syncStatus = await existingDatabase.CheckStatus(projectSystem);
            existingDatabase.SyncStatus = syncStatus;
        }
    }

    /// <summary>
    /// Checks and updates the synchronization status of an existing database with the Dropbox cloud storage.
    /// This process evaluates the current synchronization state by comparing the database on local storage and in the cloud.
    /// </summary>
    /// <param name="existingDatabase">The instance of the database to be checked for synchronization.</param>
    /// <param name="projectSystem">The type of project system initiating the sync check.</param>
    // ReSharper disable once HeapView.ClosureAllocation
    public static void CheckExistingDatabaseIsSync(this ExistingDatabase existingDatabase,
        ProjectSystem projectSystem)
    {
        // ReSharper disable once HeapView.DelegateAllocation
        Task.Run(async () =>
        {
            var syncStatus = await existingDatabase.CheckStatus(projectSystem);
            existingDatabase.SyncStatus = syncStatus;
        }).ConfigureAwait(false).GetAwaiter().GetResult();
    }
}