﻿using System.Collections.ObjectModel;

namespace MyExpenses.Utils;

/// <summary>
/// Provides extension methods for the ObservableCollection class.
/// </summary>
public static class ObservableCollectionExtensions
{
    /// <summary>
    /// Adds a collection of items to the end of the ObservableCollection.
    /// </summary>
    /// <param name="collection">The ObservableCollection<T> to add the items to.</param>
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
    /// Adds an item to the ObservableCollection and sorts it based on the specified key selector.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ObservableCollection.</typeparam>
    /// <param name="collection">The ObservableCollection to add the item to.</param>
    /// <param name="item">The item to add.</param>
    /// <param name="keySelector">A function to extract the sort key from each element.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection is null.</exception>
    public static void AddAndSort<T>(this ObservableCollection<T> collection, T item, Func<T, string> keySelector)
    {
        ArgumentNullException.ThrowIfNull(collection);

        var tempList = collection.ToList();
        tempList.Add(item);
        tempList.Sort((x, y) => string.Compare(keySelector(x), keySelector(y), StringComparison.Ordinal));

        var index = tempList.IndexOf(item);
        collection.Insert(index, item);
    }
}