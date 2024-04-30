using System.Windows;
using System.Windows.Media;

namespace MyExpenses.Wpf.Utils;

/// <summary>
/// The Utilities class provides utility methods for various operations.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Recursively finds all the visual children of a specified type in a given DependencyObject.
    /// </summary>
    /// <typeparam name="T">The type of visual children to find.</typeparam>
    /// <param name="obj">The DependencyObject to search.</param>
    /// <returns>An IEnumerable of the found visual children.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject obj) where T : DependencyObject
    {
        var children = new List<T>();
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T tchild)
            {
                children.Add(tchild);
            }

            children.AddRange(FindVisualChildren<T>(child));
        }

        return children;
    }
}