using System.Collections.ObjectModel;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;

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
    public static void AddAndSort<T>(this IList<T> collection, T oldItem, T item,
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
    public static void AddAndSort<T>(this IList<T> collection, T item, Func<T, string> keySelector)
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
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        var itemList = items.ToList();

        foreach (var item in itemList)
        {
            collection.Remove(item);
        }
    }

    /// <summary>
    /// Adds a range of items to an ObservableCollection and sorts them based on a key selector function.
    /// </summary>
    /// <param name="collection">The ObservableCollection to add the items to.</param>
    /// <param name="task">A Task containing the result of the operation, which is expected to contain an IEnumerable of T items.</param>
    /// <param name="keySelector">A function that returns the key used for sorting each item.</param>
    /// <param name="comparisonType">The type of string comparison to use for sorting. Defaults to StringComparison.OrdinalIgnoreCase.</param>
    /// <param name="logger">An optional ILogger instance to log errors. If not provided, no logging will occur.</param>
    /// <typeparam name="T">The type of items in the ObservableCollection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the collection or task is null.</exception>
    public static void AddRangeAndSort<T>(this IList<T> collection, Task<Result<IEnumerable<T>>> task,
        Func<T, string> keySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase,
        ILogger? logger = null)
    {
        if (task is { IsCompletedSuccessfully: true, Result.IsSuccess: true })
        {
            collection.AddRangeAndSort(task.Result.Value!, keySelector, comparisonType);
        }
        else
        {
            if (task.IsFaulted) logger?.LogError(task.Exception, "Error loading accounts");
            else logger?.LogError("Error loading accounts: {ErrorMessage}", task.Result.InternalMessage);
        }
    }

    /// <summary>
    /// Adds a range of items to an ObservableCollection and sorts it based on a key selector function.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="collection">The ObservableCollection to add the item to.</param>
    /// <param name="result">The Result containing the range of items to add.</param>
    /// <param name="keySelector">A function that returns the key used for sorting.</param>
    /// <param name="comparisonType">The StringComparison type for comparison, default is OrdinalIgnoreCase.</param>
    /// <param name="logger">An optional ILogger for logging errors.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection or result is null.</exception>
    public static void AddRangeAndSort<T>(this IList<T> collection, Result<IEnumerable<T>> result,
        Func<T, string> keySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase,
        ILogger? logger = null)
    {
        if (result.IsSuccess)
        {
            collection.AddRangeAndSort(result.Value!, keySelector, comparisonType);
        }
        else
        {
            logger?.LogError("Error loading accounts: {ErrorMessage}", result.InternalMessage);
        }
    }

    /// <summary>
    /// Adds a collection of items to the ObservableCollection and sorts it using the specified key selector.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T/> to add the items to.</param>
    /// <param name="items">The collection of items to add.</param>
    /// <param name="keySelector">The key selector function used to determine the sorting order.</param>
    /// <param name="comparisonType">The type of comparison to use when sorting the collection.</param>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    public static void AddRangeAndSort<T>(this IList<T> collection, IEnumerable<T> items,
        Func<T, string> keySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        foreach (var item in items)
        {
            collection.AddAndSort(item, keySelector, comparisonType);
        }
    }

    /// <summary>
    /// Adds a collection of items to the end of the ObservableCollection.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T/> to add the items to.</param>
    /// <param name="items">The collection of items to add.</param>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
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
    /// <param name="comparisonType">The type of comparison to use when sorting the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    public static void AddAndSort<T>(this IList<T> collection, T oldItem, T newItem,
        Func<T, string> keySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(collection);
        collection.Remove(oldItem);
        collection.AddAndSort(newItem, keySelector, comparisonType);
    }

    /// <summary>
    /// Adds an item to the ObservableCollection and sorts it based on the specified key selector.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    /// <param name="collection">The ObservableCollection to add the item to.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="keySelector">A function to extract the sort key from each element.</param>
    /// <param name="comparisonType">The type of comparison to use when sorting the collection.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    // ReSharper disable once HeapView.ClosureAllocation
    public static void AddAndSort<T>(this IList<T> collection, T item, Func<T, string> keySelector, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var index = collection
            .TakeWhile(x => string.Compare(keySelector(x), keySelector(item), comparisonType) < 0)
            .Count();

        collection.Insert(index, item);
    }

    /// <summary>
    /// Asynchronously loads a collection of items and adds them to an ObservableCollection,
    /// sorting the items based on a specified key selector.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="dataTask">A task representing the asynchronous operation to retrieve the collection of items.</param>
    /// <param name="collection">The ObservableCollection to which the items will be added and sorted.</param>
    /// <param name="sortKeySelector">A function to extract the sorting key from each item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task LoadAndSortAsync<T>(this Task<IEnumerable<T>> dataTask, ObservableCollection<T> collection,
        Func<T, string> sortKeySelector)
    {
        var items = await dataTask;
        collection.AddRangeAndSort(items, sortKeySelector);
    }
}