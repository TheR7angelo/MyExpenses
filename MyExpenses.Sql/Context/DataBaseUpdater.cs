using Microsoft.EntityFrameworkCore;
using MyExpenses.Sql.Migrations;
using Serilog;

namespace MyExpenses.Sql.Context;

/// <summary>
/// Provides functionality for updating and migrating the database schema to ensure it is aligned with the target application version.
/// </summary>
public static class DataBaseUpdater
{
    private static readonly IDatabaseMigration[] Migrations =
    [
        new Migration_1_0_0_To_1_1_0(),
        new Migration_1_1_0_To_1_2_0()
    ];

    /// <summary>
    /// Applies pending database migrations to update the database schema to match the target application version.
    /// </summary>
    /// <param name="connectionString">
    /// The optional connection string for the database. If not provided, the default configuration is used.
    /// </param>
    public static void ApplyMigrations(string? connectionString = null)
    {
        var needUpdate = false;

        var context = string.IsNullOrWhiteSpace(connectionString)
            ? new DataBaseContextOld()
            : new DataBaseContextOld(connectionString);

        var versionEntry = context.TVersions.First();
        var currentVersion = versionEntry.Version!;
        var targetAppVersion = DataBaseSeeder.CurrentVersion;

        if (currentVersion < targetAppVersion)
        {
            Log.Information("Starting database migration process. Current: {CurrentVersion}, Target: {TargetAppVersion}", currentVersion, targetAppVersion);
            needUpdate = true;
        }
        else
        {
            Log.Information("Database is up to date (Version {Version})", currentVersion);
        }

        while (currentVersion < targetAppVersion)
        {
            var migration = Migrations.FirstOrDefault(x => x.From == currentVersion);

            if (migration is null)
            {
                Log.Warning("No migration script found starting from version {CurrentVersion}. Migration stopped", currentVersion);
                break;
            }

            var nextVersion = migration.To;
            var sqlScript = migration.Command;

            Log.Information("Applying migration step: {FromVersion} -> {ToVersion}", migration.From, migration.To);

            using var transaction = context.Database.BeginTransaction();
            try
            {
                context.Database.ExecuteSqlRaw(sqlScript);

                context.Database.ExecuteSqlRaw(
                    "UPDATE t_version SET version = {0} WHERE id = {1}",
                    nextVersion.ToString(),
                    versionEntry.Id);

                transaction.Commit();

                Log.Information("Successfully migrated to version {Version}", nextVersion);

                currentVersion = nextVersion;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Fatal(ex, "Database migration failed at step {FromVersion} -> {ToVersion}. Transaction rolled back", migration.From, migration.To);
                throw;
            }
        }

        if (needUpdate && currentVersion == targetAppVersion)
        {
            Log.Information("Database migration completed successfully. Final version: {Version}", currentVersion);
        }
    }
}