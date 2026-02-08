using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MyExpenses.Sql.Context;

/// <summary>
/// Provides functionality for updating and migrating the database schema to ensure it is aligned with the target application version.
/// </summary>
public static class DataBaseUpdater
{
    /// <summary>
    /// Represents a collection of predefined SQL migration scripts used to update the database schema
    /// from one application version to another.
    /// Each key in the dictionary is a tuple containing the source version and target version,
    /// and the corresponding value is the SQL script to execute for that migration step.
    /// </summary>
    private static readonly Dictionary<(Version, Version), string> MigrationScripts = new()
    {
        [ ( new Version(1, 0, 0), new Version(1, 1, 0)) ] = MigrateFrom100To110,
    };

    private const string MigrateFrom100To110 = @"DROP TABLE IF EXISTS t_supported_languages";

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
            ? new DataBaseContext()
            : new DataBaseContext(connectionString);

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
            var migrationKey = MigrationScripts.Keys
                .FirstOrDefault(k => k.Item1 == currentVersion);

            if (migrationKey.Item1 == null)
            {
                Log.Warning("No migration script found starting from version {CurrentVersion}. Migration stopped", currentVersion);
                break;
            }

            var nextVersion = migrationKey.Item2;
            var sqlScript = MigrationScripts[migrationKey];

            Log.Information("Applying migration step: {FromVersion} -> {ToVersion}", migrationKey.Item1, migrationKey.Item2);

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
                Log.Fatal(ex, "Database migration failed at step {FromVersion} -> {ToVersion}. Transaction rolled back", migrationKey.Item1, migrationKey.Item2);
                throw;
            }
        }

        if (needUpdate && currentVersion == targetAppVersion)
        {
            Log.Information("Database migration completed successfully. Final version: {Version}", currentVersion);
        }
    }
}