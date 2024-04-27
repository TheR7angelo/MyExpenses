using System.Windows;
using System.Windows.Media;

namespace MyExpenses.Wpf.Utils;

public static class Utilities
{
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