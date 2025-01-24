using MyExpenses.Models.Sql;

namespace MyExpenses.Sql.Context;

public static class DbContextHelperConverter
{
    /// <summary>
    /// Converts an integer to an ISql object of type T.
    /// </summary>
    /// <typeparam name="T">The type of ISql object to convert to.</typeparam>
    /// <param name="id">The id of the ISql object.</param>
    /// <returns>The ISql object of type T with the specified id, or null if no ISql object is found with the id.</returns>
    public static T? ToISql<T>(this int id) where T : class, ISql
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This method iterates through all existing databases to apply updates using their respective contexts.
        // The use of `UpdateAllDefaultValues()` is necessary to ensure the default values remain consistent
        // across all databases, and `SaveChanges()` is called to persist these changes. This approach is essential
        // given the need to handle multiple databases in a controlled and sequential manner.
        using var context = new DataBaseContext();
        var result = context.Set<T>().FirstOrDefault(s => s.Id == id);

        return result;
    }
}