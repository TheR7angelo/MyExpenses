namespace MyExpenses.Models.WebApi.Dropbox;

/// <summary>
/// Represents the synchronization status of a database or file in a Dropbox integration context.
/// </summary>
public enum SyncStatus
{
    Unknown,
    UnSynchronized,
    LocalIsOutdated,
    RemoteIsOutdated,
    Synchronized
}