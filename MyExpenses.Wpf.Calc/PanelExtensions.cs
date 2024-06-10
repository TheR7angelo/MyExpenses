using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Calc;

/// <summary>
/// Provides attached properties for the <see cref="Panel"/> class.
/// </summary>
public static class PanelExtensions
{
    /// <summary>
    /// Provides an attached property for setting the margin for all children of a <see cref="Panel"/>.
    /// </summary>
    public static readonly DependencyProperty MarginForAllChildrenProperty =
        DependencyProperty.RegisterAttached("MarginForAllChildren", typeof(Thickness),
            typeof(PanelExtensions), new PropertyMetadata(default(Thickness), OnMarginForAllChildrenChanged));

    /// <summary>
    /// Provides an attached property for setting the margin for all children of a <see cref="Panel"/>.
    /// </summary>
    /// <param name="panel">The Panel instance to set the margin for its children.</param>
    /// <param name="value">The margin value to be set for all children.</param>
    public static void SetMarginForAllChildren(this Panel panel, Thickness value) =>
        panel.SetValue(MarginForAllChildrenProperty, value);

    /// <summary>
    /// Gets the margin value set for all children of a <see cref="Panel"/>.
    /// </summary>
    /// <param name="panel">The Panel instance to get the margin for its children.</param>
    /// <returns>The margin value set for all children.</returns>
    public static Thickness GetMarginForAllChildren(this Panel panel) =>
        (Thickness)panel.GetValue(MarginForAllChildrenProperty);

    /// <summary>
    /// Handler for the <see cref="PanelExtensions.MarginForAllChildrenProperty"/> property changed event.
    /// Applies the margin value to all children of the panel.
    /// </summary>
    /// <param name="d">The dependency object representing the panel.</param>
    /// <param name="e">The event arguments containing the new margin value.</param>
    private static void OnMarginForAllChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel panel)
        {
            panel.Loaded += (_, _) => ApplyMargin(panel, (Thickness)e.NewValue);
        }
    }

    /// <summary>
    /// Applies the specified margin to all children of the specified <see cref="Panel"/>.
    /// </summary>
    /// <param name="panel">The <see cref="Panel"/> instance to apply the margin to its children.</param>
    /// <param name="margin">The margin value to be applied to all children.</param>
    private static void ApplyMargin(Panel panel, Thickness margin)
    {
        foreach (FrameworkElement child in panel.Children)
        {
            if (child.Margin == default)
            {
                child.Margin = margin;
            }
        }
    }
}