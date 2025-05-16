using System.Collections.ObjectModel;
using MyExpenses.Models.IO;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Dropbox;

namespace MyExpenses.Core;

public static class ImportExportUtils
{
    /// <summary>
    /// Refreshes the collection of existing databases by removing non-existent database files
    /// and adding or updating databases based on the current state of the file system.
    /// </summary>
    /// <param name="existingDatabases">The collection of existing databases to be refreshed.</param>
    /// <param name="projectSystem">Specifies the project system, such as Wpf or Maui, for handling specific operations.</param>
    public static void RefreshExistingDatabases(this ObservableCollection<ExistingDatabase> existingDatabases,
        ProjectSystem projectSystem)
    {
        var itemsToDelete = existingDatabases
            .Where(s => !File.Exists(s.FilePath)).ToArray();

        foreach (var item in itemsToDelete)
        {
            existingDatabases.Remove(item);
        }

        var newExistingDatabases = DbContextBackup.GetExistingDatabase();

        // ReSharper disable once HeapView.ClosureAllocation
        foreach (var existingDatabase in newExistingDatabases)
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var exist = existingDatabases.FirstOrDefault(s => s.FilePath == existingDatabase.FilePath);
            if (exist is not null)
            {
                existingDatabase.CopyPropertiesTo(exist);
            }
            else
            {
                existingDatabases.AddAndSort(existingDatabase, s => s.FileNameWithoutExtension);
            }
        }

        _ = existingDatabases.CheckExistingDatabaseIsSyncAsync(projectSystem);
    }
}