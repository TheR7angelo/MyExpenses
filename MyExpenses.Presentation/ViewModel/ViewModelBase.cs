using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Dependencies;
using MyExpenses.Presentation.Messages;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Base class for view models in the application.
/// Provides methods for handling entity changes and item deletions, facilitating data management in views.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Handles entity changes for the specified collection and type.
    /// </summary>
    /// <param name="m">The EntityChangedMessage containing the change data.</param>
    /// <param name="collection">The ObservableCollection to update.</param>
    /// <param name="expectedType">The expected DependencyType of the entity.</param>
    /// <param name="getId">A function to get the unique identifier of an entity.</param>
    /// <param name="getName">A function to get the display name of an entity.</param>
    /// <param name="merge">An action to merge changes from the source entity to the target entity.</param>
    /// <param name="update">An action to update the ViewModel after a change.</param>
    /// <typeparam name="T">The type of entity being changed.</typeparam>
    private protected void OnEntityChanged<T>(
        EntityChangedMessage<T> m,
        ObservableCollection<T> collection,
        DependencyType expectedType,
        Func<T, int> getId,
        Func<T, string?> getName,
        Action<T, T> merge,
        Action<T> update) where T : class
    {
        if (m.Value.EntityType != expectedType) return;

        var content = m.Value.Content;

        switch (m.Value.DataAction)
        {
            case DataAction.Update:
                var item = collection.FirstOrDefault(s => getId(s) == getId(content));
                if (item is not null)
                    merge(content, item);
                break;

            case DataAction.Add:
                collection.AddAndSort(content, s => getName(s)!);
                update(content);
                break;
        }
    }

    /// <summary>
    /// Handles item deletions from the specified collection and type.
    /// </summary>
    /// <param name="m">The EntityChangedMessage containing the change data.</param>
    /// <param name="collection">The IList to update.</param>
    /// <param name="expectedType">The expected DependencyType of the item.</param>
    /// <param name="getId">A function to get the unique identifier of an item.</param>
    /// <typeparam name="T">The type of item being deleted.</typeparam>
    private protected void OnItemDeleted<T>(EntityChangedMessage<int[]> m,
        IList<T> collection,
        DependencyType expectedType,
        Func<T, int> getId)
    {
        if (m.Value.DataAction is not DataAction.Delete) return;
        if (m.Value.EntityType != expectedType) return;

        var ids = m.Value.Content;
        var toDeletes = collection.Where(s => ids.Contains(getId(s))).ToList();
        collection.RemoveRange(toDeletes);
    }
}