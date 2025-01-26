using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Attributs;
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Ignoring the performance hint regarding the use of StringBuilder for constructing the connection string.
        // The use of List<string> here is deliberate for better readability and maintainability, especially
        // given that the list of parameters is relatively small and the performance difference is negligible.
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

    /// <summary>
    /// Executes a raw SQL query asynchronously using Entity Framework and returns the number of rows affected.
    /// </summary>
    /// <param name="sql">The raw SQL query to execute. The caller is responsible for ensuring the validity and safety of the query.</param>
    /// <param name="tempFilePath">Optional parameter specifying a temporary file path for creating the database context. If null, the default data source is used.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the sql parameter is null or empty.</exception>
    /// <exception cref="DbUpdateException">Thrown if an error occurs while attempting to perform the SQL operation.</exception>
    public static async Task<int> ExecuteRawSqlAsync(this string sql, string? tempFilePath = null)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using raw SQL execution here is necessary due to the specific nature of the operation that cannot be
        // performed with Entity Framework's LINQ or other abstractions. The responsibility of ensuring the safety
        // and correctness of the SQL query lies with the caller.
        await using var context = new DataBaseContext(tempFilePath);
        return await context.Database.ExecuteSqlRawAsync(sql);
    }

    /// <summary>
    /// Executes a raw SQL statement against the database using a specific database context.
    /// </summary>
    /// <param name="sql">The raw SQL command to execute. It is the caller's responsibility to ensure the safety and correctness of the SQL syntax.</param>
    /// <param name="tempFilePath">The file path of the database to use for this operation. If null, the default database context will be used.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided SQL command is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the database context cannot be initialized properly.</exception>
    /// <exception cref="DbUpdateException">Thrown if there is an error executing the SQL command.</exception>
    public static int ExecuteRawSql(this string sql, string? tempFilePath = null)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using raw SQL execution here is necessary due to the specific nature of the operation that cannot be
        // performed with Entity Framework's LINQ or other abstractions. The responsibility of ensuring the safety
        // and correctness of the SQL query lies with the caller.
        using var context = new DataBaseContext(tempFilePath);
        return context.Database.ExecuteSqlRaw(sql);
    }

    /// <summary>
    /// Deletes an entity of type <typeparamref name="T"/> from the database, with an option for cascading deletions.
    /// </summary>
    /// <typeparam name="T">The entity type that implements the <see cref="ISql"/> interface.</typeparam>
    /// <param name="entity">The entity to delete from the database.</param>
    /// <param name="cascade">Specifies whether to perform cascading deletions. Default is false.</param>
    /// <returns>A tuple indicating the success of the operation and any exception that may have occurred.
    /// The first item is a boolean representing success; the second is an exception if an error occurred.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided entity is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the database context cannot perform the delete operation.</exception>
    public static (bool Success, Exception? Exception) Delete<T>(this T entity, bool cascade = false)
        where T : class, ISql
    {
        try
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // This implementation requires direct deletion of the entity due to specific application logic and
            // the need to handle cascading deletions explicitly. The use of `SaveChanges()` ensures the operation
            // is committed immediately, and the exception handling guarantees robustness in case of failure.
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

    /// <summary>
    /// Deletes an entity from the DbContext, with optional cascade deletion.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> used to manage the data operations.</param>
    /// <param name="entity">The target entity to be deleted.</param>
    /// <param name="predicate">
    /// An expression defining the condition to locate an existing entity.
    /// Used only when cascade deletion is disabled.
    /// </param>
    /// <param name="cascade">
    /// Determines whether the deletion should be cascaded.
    /// If enabled, all related entities (via navigation properties) will also be deleted.
    /// </param>
    /// <remarks>
    /// When cascade deletion is enabled, all navigation collections of the entity are loaded,
    /// and their child entities are also deleted.
    /// Otherwise, the deletion is conditional, depending on the result of locating a matching entity.
    /// </remarks>
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

    /// <summary>
    /// Adds a new entity or updates an existing one in the database.
    /// </summary>
    /// <typeparam name="T">The type of the entity, which must implement the ISql interface.</typeparam>
    /// <param name="entity">The entity to be added or updated in the database.</param>
    /// <returns>A tuple containing a success flag and an exception if an error occurs. The success value is true if the operation was successful, otherwise false. If the operation fails, the exception will contain details of the error.</returns>
    public static (bool Success, Exception? Exception) AddOrEdit<T>(this T entity) where T : class, ISql
    {
        try
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // This method implements an upsert operation, which directly handles adding or updating the entity based on its existence.
            // The use of `SaveChanges()` ensures that modifications are committed immediately, and exception handling provides robustness
            // in case the operation fails. The approach is necessary to fulfill specific application requirements.
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

    /// <summary>
    /// Performs an Upsert operation, which adds the entity to the DbContext if it does not already exist,
    /// otherwise updates the existing entity based on the specified predicate.
    /// </summary>
    /// <param name="context">The DbContext instance used to perform the Upsert operation.</param>
    /// <param name="entity">The entity to be added or updated in the database.</param>
    /// <param name="predicate">A lambda expression to identify whether the entity already exists in the database.</param>
    /// <typeparam name="TEntity">The type of the entity to be added or updated. Must be a reference type.</typeparam>
    private static void Upsert<TEntity>(this DbContext context, TEntity entity,
        Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var existingEntity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);

        if (existingEntity is null) context.Set<TEntity>().Add(entity);
        else context.Set<TEntity>().Update(entity);
    }

    /// <summary>
    /// Loads all related collections for a given entity in the DbContext.
    /// </summary>
    /// <param name="context">The DbContext instance used to track entities and their state.</param>
    /// <param name="entity">The entity for which related collections will be loaded.</param>
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

    /// <summary>
    /// Retrieves the navigation properties of a specified entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="entity">The entity for which to retrieve the navigation properties.</param>
    /// <returns>A collection of PropertyInfo objects representing the navigation properties of the entity.</returns>
    private static IEnumerable<PropertyInfo> GetNavigationProperty<T>(this T entity)
    {
        var properties = entity!.GetType().GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
            ).ToArray();
        return properties;
    }

    /// <summary>
    /// Updates the default language values for all existing databases. This method retrieves
    /// all existing database file paths, updates their default values to align with the current
    /// application language or configuration, and saves the changes to each database.
    /// </summary>
    public static void UpdateDbLanguage()
    {
        var existingDatabases = DbContextBackup.GetExistingDatabase();
        foreach (var existingDatabase in existingDatabases)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // This method is required to update all default values across multiple databases by iterating through them.
            // Using `SaveChanges()` ensures that the updates are persisted immediately to each database. The approach
            // is necessary to maintain consistency across all database instances, and the use of a loop handles multiple
            // database contexts sequentially.
            using var context = new DataBaseContext(existingDatabase.FilePath);
            _ = context.UpdateAllDefaultValues();
            context.SaveChanges();
        }
    }

    /// <summary>
    /// Retrieves the table or view name for the specified entity type from the database context.
    /// </summary>
    /// <param name="context">The database context used to access model metadata.</param>
    /// <param name="tableType">The type of the entity for which to retrieve the table or view name.</param>
    /// <returns>The name of the table or view associated with the specified entity type, or null if none is found.</returns>
    public static string? GetTableName(this DataBaseContext context, Type tableType)
    {
        var tableName = context.Model.FindEntityType(tableType)?.GetTableName();
        var viewName = context.Model.FindEntityType(tableType)?.GetViewName();

        return tableName ?? viewName;
    }

    /// <summary>
    /// Resets all writable properties of the object implementing ISql to their default values.
    /// </summary>
    /// <param name="iSql">The object implementing the ISql interface to be reset.</param>
    public static void Reset(this ISql iSql)
    {
        iSql.DetachEntity();

        var properties = iSql.GetType().GetProperties()
            .Where(p => p.CanWrite && p.GetCustomAttributes(typeof(IgnoreResetAttribute), true).Length is 0);
        foreach (var property in properties)
        {
            // object? defaultValue;
            // if (property.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()))
            // {
            //     var itemType = property.PropertyType.GetGenericArguments()[0];
            //     defaultValue = Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
            // }
            // else
            // {
            var defaultValue = property.PropertyType.IsValueType
                ? Activator.CreateInstance(property.PropertyType)
                : null;
            // }

            property.SetValue(iSql, defaultValue);

        }

        if (iSql is IDefaultBehavior defaultBehavior) defaultBehavior.SetDefaultValues();
    }

    private static void DetachEntity(this ISql iSql)
    {
        using var context = new DataBaseContext();
        context.Entry(iSql).State = EntityState.Detached;
    }
}