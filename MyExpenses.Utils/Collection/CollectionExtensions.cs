namespace MyExpenses.Utils.Collection;

/// <summary>
/// A static class that provides extension methods for working with collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Adds an item to a collection and sorts it based on a key selector function.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The collection to add the item to.</param>
    /// <param name="oldItem">The item to remove from the collection before adding the new item.</param>
    /// <param name="item">The item to add to the collection.</param>
    /// <param name="keySelector">A function that returns the key used for sorting.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    public static void AddAndSort<T>(this List<T> collection, T oldItem, T item,
        Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.Remove(oldItem);
        collection.AddAndSort(item, keySelector);
    }

    /// <summary>
    /// Adds an item to a collection and sorts it based on a key selector function.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The collection to add the item to.</param>
    /// <param name="item">The item to add to the collection.</param>
    /// <param name="keySelector">A function that returns the key used for sorting.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    public static void AddAndSort<T>(this List<T> collection, T item, Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var tempList = collection.ToList();
        tempList.Add(item);
        tempList.Sort((x, y) => string.Compare(keySelector(x), keySelector(y), StringComparison.Ordinal));

        var index = tempList.IndexOf(item);
        collection.Insert(index, item);
    }
}