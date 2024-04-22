using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf;

public static class PanelExtensions
{
    public static readonly DependencyProperty MarginForAllChildrenProperty =
        DependencyProperty.RegisterAttached("MarginForAllChildren", typeof(Thickness),
            typeof(PanelExtensions), new PropertyMetadata(default(Thickness), OnMarginForAllChildrenChanged));

    public static void SetMarginForAllChildren(this Panel panel, Thickness value) =>
        panel.SetValue(MarginForAllChildrenProperty, value);

    public static Thickness GetMarginForAllChildren(this Panel panel) =>
        (Thickness)panel.GetValue(MarginForAllChildrenProperty);

    private static void OnMarginForAllChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel panel)
        {
            panel.Loaded += (_, _) => ApplyMargin(panel, (Thickness)e.NewValue);
        }
    }

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