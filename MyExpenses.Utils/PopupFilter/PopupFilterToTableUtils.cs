using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Utils.PopupFilter;

public static class PopupFilterToTableUtils
{
    /// <summary>
    /// Converts a collection of <see cref="PopupSearch"/> items to a table of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the collection should be converted.</typeparam>
    /// <param name="items">The collection of <see cref="PopupSearch"/> items to be converted.</param>
    /// <returns>An array of the specified type containing the converted items, or null if the type conversion fails.</returns>
    /// <exception cref="NotImplementedException">Thrown if the specified type is not supported.</exception>
    public static T[]? ToTable<T>(this IEnumerable<PopupSearch> items)
    {
        using var context = new DataBaseContext();

        var type = typeof(T);
        if (type == typeof(TAccount))
        {
            return items.Select(s => context.TAccounts.First(a => a.Id.Equals(s.Id))).ToArray() as T[];
        }

        throw new NotImplementedException($"Conversion to type '{typeof(T).Name}' is not supported in the method {nameof(ToTable)}");
    }
}