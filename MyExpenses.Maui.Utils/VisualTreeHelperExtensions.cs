using System.Runtime.Versioning;

namespace MyExpenses.Maui.Utils;

public static class VisualTreeHelperExtensions
{
    /// <summary>
    /// Recursively retrieves all visual children of a specific type from a given element.
    /// </summary>
    /// <typeparam name="T">The type of visual children to search for.</typeparam>
    /// <param name="element">The parent element from which to start the search.</param>
    /// <returns>An enumerable collection of all children of the given type found within the visual tree.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(this Element? element) where T : Element
    {
        if (element is not IVisualTreeElement visualElement) yield break;

        foreach (var child in visualElement.GetVisualChildren())
        {
            if (child is T tChild)
            {
                yield return tChild;
            }

            foreach (var childOfChild in FindVisualChildren<T>(child as Element))
            {
                yield return childOfChild;
            }
        }
    }

    /// <summary>
    /// Searches the visual tree of a specified element for the first instance of a specific type and returns it.
    /// If no such element is found in the visual tree, searches upward in the parent hierarchy for the first instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the element to find in the visual tree or parent hierarchy.</typeparam>
    /// <param name="element">The starting element to search within the visual tree and parent hierarchy.</param>
    /// <returns>The first element of the specified type found either in the visual tree or in the parent hierarchy, or null if no such element exists.</returns>
    public static T? FindVisualChild<T>(this Element? element) where T : Element
    {
        var result = element.FindInVisualTree<T>();
        return result ?? element.FindInParentHierarchy<T>();
    }

    /// <summary>
    /// Searches the visual tree of a specified element recursively and retrieves the first instance of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the element to find in the visual tree.</typeparam>
    /// <param name="element">The starting element to search within the visual tree.</param>
    /// <returns>The first element of the specified type found within the visual tree, or null if no such element exists.</returns>
    private static T? FindInVisualTree<T>(this Element? element) where T : Element
    {
        if (element is not IVisualTreeElement visualElement) return null;

        foreach (var child in visualElement.GetVisualChildren())
        {
            if (child is T tChild) return tChild;

            var childOfChild = FindInVisualTree<T>(child as Element);
            if (childOfChild is not null) return childOfChild;
        }

        return null;
    }

    /// <summary>
    /// Searches recursively upwards in the parent hierarchy to find the first parent of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of parent to search for.</typeparam>
    /// <param name="element">The element from which to start the search.</param>
    /// <returns>The first parent of the given type found in the hierarchy, or null if no such parent exists.</returns>
    public static T? FindInParentHierarchy<T>(this Element? element) where T : Element
    {
        var parent = element?.Parent;

        while (parent is not null)
        {
            if (parent is T tParent) return tParent;

            parent = parent.Parent;
        }

        return null;
    }
}