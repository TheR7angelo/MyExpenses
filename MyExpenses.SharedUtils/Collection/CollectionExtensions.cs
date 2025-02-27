using System.Collections.ObjectModel;

namespace MyExpenses.SharedUtils.Collection;

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
    // ReSharper disable once HeapView.ClosureAllocation
    public static void AddAndSort<T>(this List<T> collection, T item, Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var tempList = collection.ToList();
        tempList.Add(item);

        // ReSharper disable once HeapView.DelegateAllocation
        tempList.Sort((x, y) => string.Compare(keySelector(x), keySelector(y), StringComparison.Ordinal));

        var index = tempList.IndexOf(item);
        collection.Insert(index, item);
    }

    /// <summary>
    /// Removes a collection of items from the ObservableCollection.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T/> to remove the items from.</param>
    /// <param name="items">The collection of items to remove.</param>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        var itemList = items.ToList();

        foreach (var item in itemList)
        {
            collection.Remove(item);
        }
    }

    /// <summary>
    /// Adds a collection of items to the ObservableCollection and sorts it using the specified key selector.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T/> to add the items to.</param>
    /// <param name="items">The collection of items to add.</param>
    /// <param name="keySelector">The key selector function used to determine the sorting order.</param>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    public static void AddRangeAndSort<T>(this ObservableCollection<T> collection, IEnumerable<T> items,
        Func<T, string> keySelector)
    {
        foreach (var item in items)
        {
            collection.AddAndSort(item, keySelector);
        }
    }

    /// <summary>
    /// Adds a collection of items to the end of the ObservableCollection.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T/> to add the items to.</param>
    /// <param name="items">The collection of items to add.</param>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Adds an item to the ObservableCollection and keeps the collection sorted based on a key selector.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    /// <param name="collection">The ObservableCollection to add the item to.</param>
    /// <param name="oldItem">The old item to be removed from the collection.</param>
    /// <param name="newItem">The item to be added to the collection.</param>
    /// <param name="keySelector">A function that selects a key for sorting the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    public static void AddAndSort<T>(this ObservableCollection<T> collection, T oldItem, T newItem,
        Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.Remove(oldItem);
        collection.AddAndSort(newItem, keySelector);
    }

    /// <summary>
    /// Adds an item to the ObservableCollection and sorts it based on the specified key selector.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    /// <param name="collection">The ObservableCollection to add the item to.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="keySelector">A function to extract the sort key from each element.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    // ReSharper disable once HeapView.ClosureAllocation
    public static void AddAndSort<T>(this ObservableCollection<T> collection, T item, Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var tempList = collection.ToList();
        tempList.Add(item);

        // ReSharper disable once HeapView.DelegateAllocation
        tempList.Sort((x, y) => string.Compare(keySelector(x), keySelector(y), StringComparison.Ordinal));

        var index = tempList.IndexOf(item);
        collection.Insert(index, item);
    }
}