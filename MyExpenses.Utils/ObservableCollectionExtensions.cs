using System.Collections.ObjectModel;

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
}