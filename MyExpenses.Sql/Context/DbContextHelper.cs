using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql;

namespace MyExpenses.Sql.Context;

public static class DbContextHelper
{
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
}