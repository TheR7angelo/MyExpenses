using System.Runtime.Versioning;

namespace MyExpenses.Maui.Utils;

public static class VisualTreeHelperExtensions
{
    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst15.0")]
    [SupportedOSPlatform("Windows")]
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
}