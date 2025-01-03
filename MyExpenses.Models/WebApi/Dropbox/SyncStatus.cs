namespace MyExpenses.Models.WebApi.DropBox;

/// <summary>
/// Represents the synchronization status of a database or file in a Dropbox integration context.
/// </summary>
public enum SyncStatus
{
    LocalIsOutdated,
    RemoteIsOutdated,
    Synchronized
}