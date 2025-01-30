using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyExpenses.Wpf.Utils;

/// <summary>
/// The Utilities class provides utility methods for various operations.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Finds the TabItem in the visual tree of a specified TabControl that has a matching header.
    /// </summary>
    /// <param name="tabControl">The TabControl to search.</param>
    /// <param name="header">The header text to match.</param>
    /// <returns>The TabItem with a matching header, or null if not found.</returns>
    public static TabItem? FindTabItemByHeader(this TabControl tabControl, string header)
    {
        var children = tabControl.FindVisualChildren<TabItem>();
        return children.FirstOrDefault(child => child.Header.Equals(header));
    }

    /// <summary>
    /// Recursively finds the first child of a specified type in the visual tree of a given DependencyObject.
    /// </summary>
    /// <typeparam name="T">The type of child to find.</typeparam>
    /// <param name="parent">The DependencyObject to search.</param>
    /// <returns>The first child of the specified type found in the visual tree, or null if not found.</returns>
    public static T? FindChild<T>(this DependencyObject parent) where T : DependencyObject
    {
        if (parent is T dependencyObject)
        {
            return dependencyObject;
        }

        T? child = null;

        for (var i = 0; child == null && i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            child = FindChild<T>(VisualTreeHelper.GetChild(parent, i));
        }

        return child;
    }

    /// <summary>
    /// Recursively finds all the visual children of a specified type in a given DependencyObject.
    /// </summary>
    /// <typeparam name="T">The type of visual children to find.</typeparam>
    /// <param name="obj">The DependencyObject to search.</param>
    /// <returns>An IEnumerable of the found visual children.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject obj) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T tchild)
            {
                yield return tchild;
            }

            foreach (var descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }

    // public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject obj) where T : DependencyObject
    // {
    //     var children = new List<T>();
    //     for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
    //     {
    //         var child = VisualTreeHelper.GetChild(obj, i);
    //         if (child is T tchild)
    //         {
    //             children.Add(tchild);
    //         }
    //
    //         children.AddRange(FindVisualChildren<T>(child));
    //     }
    //
    //     return children;
    // }
}