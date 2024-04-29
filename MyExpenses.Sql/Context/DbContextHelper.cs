using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Sql.Context;

public static class DbContextHelper
{
    public static (bool Success, Exception? exception) AddOrEdit(this TAccountType accountType)
    {
        try
        {
            using var context = new DataBaseContext();
            context.Upsert(accountType, s => s.Id == accountType.Id);
            context.SaveChanges();
            return (true, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, e);
        }
    }

    public static (bool Success, Exception? exception) AddOrEdit(this TAccount account)
    {
        try
        {
            using var context = new DataBaseContext();
            context.Upsert(account, s => s.Id == account.Id);
            context.SaveChanges();
            return (true, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, e);
        }
    }

    public static (bool Success, Exception? exception) AddOrEdit(this TPlace place)
    {
        try
        {
            using var context = new DataBaseContext();
            context.Upsert(place, s => s.Id == place.Id);
            context.SaveChanges();
            return (true, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, e);
        }
    }

    public static void Upsert<TEntity>(this DbContext context, TEntity entity, Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var existingEntity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);

        if (existingEntity is null) context.Set<TEntity>().Add(entity);
        else context.Set<TEntity>().Update(entity);
    }
}