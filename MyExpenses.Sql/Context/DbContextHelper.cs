using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql;

namespace MyExpenses.Sql.Context;

public static class DbContextHelper
{
    /// <summary>
    /// Builds a SQLite connection string based on the provided parameters.
    /// </summary>
    /// <param name="dataSource">The file path to the SQLite database.</param>
    /// <param name="mode">Specifies the mode in which to open the SQLite database. Default is ReadWrite.</param>
    /// <param name="pooling">Determines whether connection pooling is enabled.</param>
    /// <param name="cache">Specifies the cache mode for the SQLite connection. Default is Default.</param>
    /// <param name="password">The password to secure the SQLite database, if applicable.</param>
    /// <param name="foreignKeys">Specifies whether foreign key constraints should be enforced.</param>
    /// <param name="recursiveTriggers">Determines whether recursive triggers are enabled.</param>
    /// <param name="browsableConnectionString">Determines whether the connection string is included in diagnostic output.</param>
    /// <param name="defaultTimeout">Sets the default timeout value for database operations.</param>
    /// <returns>A fully configured SQLite connection string.</returns>
    /// <exception cref="ArgumentException">Thrown if the dataSource is null, empty, or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the specified dataSource file doesn't exist.</exception>
    internal static string BuildConnectionString(this string dataSource,
        SqliteOpenMode mode = SqliteOpenMode.ReadWrite,
        bool pooling = false,
        SqliteCacheMode cache = SqliteCacheMode.Default,
        string? password = null,
        bool? foreignKeys = null,
        bool recursiveTriggers = false,
        bool browsableConnectionString = false,
        int defaultTimeout = 0)
    {
        if (string.IsNullOrWhiteSpace(dataSource))
            throw new ArgumentException(@"DataSource cannot be null or empty", nameof(dataSource));
        if (!File.Exists(dataSource)) throw new FileNotFoundException("DataSource does not exist", dataSource);

        var parts = new List<string>
        {
            $"Data Source=\"{dataSource}\"",
            $"Mode={mode}",
            $"Pooling={pooling.ToString()}"
        };

        if (cache is not SqliteCacheMode.Default) parts.Add($"Cache={cache}");
        if (!string.IsNullOrEmpty(password)) parts.Add($"Password={password}");
        if (foreignKeys.HasValue) parts.Add($"Foreign Keys={foreignKeys.Value}");
        if (recursiveTriggers) parts.Add($"Recursive Triggers={recursiveTriggers.ToString()}");
        if (browsableConnectionString) parts.Add($"Browsable Connection String={browsableConnectionString.ToString()}");
        if (defaultTimeout is not 0) parts.Add($"Default Timeout={defaultTimeout}");

        return string.Join(";", parts);
    }

    public static async Task<int> ExecuteRawSqlAsync(this string sql, string? tempFilePath = null)
    {
        await using var context = new DataBaseContext(tempFilePath);
        return await context.Database.ExecuteSqlRawAsync(sql);
    }

    public static int ExecuteRawSql(this string sql, string? tempFilePath = null)
    {
        using var context = new DataBaseContext(tempFilePath);
        return context.Database.ExecuteSqlRaw(sql);
    }

    public static (bool Success, Exception? Exception) Delete<T>(this T entity, bool cascade = false)
        where T : class, ISql
    {
        try
        {
            using var context = new DataBaseContext();
            context.Delete(entity, s => s.Id == entity.Id, cascade);
            context.SaveChanges();
            return (true, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, e);
        }
    }

    private static void Delete<TEntity>(this DbContext context, TEntity entity,
        Expression<Func<TEntity, bool>> predicate, bool cascade) where TEntity : class
    {
        if (cascade)
        {
            context.LoadAllCollections(entity);
            var properties = entity.GetNavigationProperty();
            var children = properties
                .Where(property => property.GetValue(entity) is IList && (property.GetValue(entity) as IList)!.Count > 0)
                .SelectMany(property => (property.GetValue(entity) as IList)!.OfType<ISql>())
                .ToList();

            foreach (var child in children) child.Delete(cascade);

            context.Entry(entity).State = EntityState.Deleted;
        }
        else
        {
            var existingEntity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);

            if (existingEntity is null) return;
            context.Set<TEntity>().Remove(entity);
        }
    }

    public static (bool Success, Exception? Exception) AddOrEdit<T>(this T entity) where T : class, ISql
    {
        try
        {
            using var context = new DataBaseContext();
            context.Upsert(entity, s => s.Id == entity.Id);
            context.SaveChanges();
            return (true, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, e);
        }
    }

    private static void Upsert<TEntity>(this DbContext context, TEntity entity,
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var existingEntity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);

        if (existingEntity is null) context.Set<TEntity>().Add(entity);
        else context.Set<TEntity>().Update(entity);
    }

    private static void LoadAllCollections(this DbContext context, object entity)
    {
        var entry = context.Entry(entity);

        var properties = entity.GetNavigationProperty();

        foreach (var navigationProperty in properties)
        {
            var collection = entry.Collection(navigationProperty.Name);
            if (!collection.IsLoaded)
                collection.Load();
        }
    }

    private static IEnumerable<PropertyInfo> GetNavigationProperty<T>(this T entity)
    {
        var properties = entity!.GetType().GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ).ToArray();
        return properties;
    }

    public static void UpdateDbLanguage()
    {
        var newExistingDatabases = DbContextBackup.GetExistingDatabase();
        foreach (var newExistingDatabase in newExistingDatabases)
        {
            using var context = new DataBaseContext(newExistingDatabase.FilePath);
            _ = context.UpdateAllDefaultValues();
            context.SaveChanges();
        }
    }

    public static string? GetTableName(this DataBaseContext context, Type tableType)
    {
        var tableName = context.Model.FindEntityType(tableType)?.GetTableName();
        var viewName = context.Model.FindEntityType(tableType)?.GetViewName();

        return tableName ?? viewName;
    }
}