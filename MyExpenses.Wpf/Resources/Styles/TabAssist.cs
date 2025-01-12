using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Resources.Styles;

public static class TabAssist
{
    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty HasFilledTabProperty = DependencyProperty.RegisterAttached(
        "HasFilledTab", typeof(bool), typeof(TabAssist), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public static void SetHasFilledTab(DependencyObject element, bool value)
        => element.SetValue(HasFilledTabProperty, value);

    public static bool GetHasFilledTab(DependencyObject element)
        => (bool)element.GetValue(HasFilledTabProperty);

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty HasUniformTabWidthProperty = DependencyProperty.RegisterAttached(
        "HasUniformTabWidth", typeof(bool), typeof(TabAssist), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public static void SetHasUniformTabWidth(DependencyObject element, bool value)
        => element.SetValue(HasUniformTabWidthProperty, value);

    public static bool GetHasUniformTabWidth(DependencyObject element)
        => (bool)element.GetValue(HasUniformTabWidthProperty);

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty HeaderPanelMarginProperty = DependencyProperty.RegisterAttached(
        "HeaderPanelMargin", typeof(Thickness), typeof(TabAssist), new PropertyMetadata(default(Thickness)));

    // ReSharper disable once HeapView.BoxingAllocation
    public static void SetHeaderPanelMargin(DependencyObject element, Thickness value)
        => element.SetValue(HeaderPanelMarginProperty, value);

    public static Thickness GetHeaderPanelMargin(DependencyObject element)
        => (Thickness) element.GetValue(HeaderPanelMarginProperty);

    internal static Visibility GetBindableIsItemsHost(DependencyObject obj)
        => (Visibility)obj.GetValue(BindableIsItemsHostProperty);

    // ReSharper disable once HeapView.BoxingAllocation
    internal static void SetBindableIsItemsHost(DependencyObject obj, Visibility value)
        => obj.SetValue(BindableIsItemsHostProperty, value);

    // ReSharper disable once HeapView.BoxingAllocation
    internal static readonly DependencyProperty BindableIsItemsHostProperty =
        DependencyProperty.RegisterAttached("BindableIsItemsHost", typeof(Visibility), typeof(TabAssist), new PropertyMetadata(Visibility.Collapsed, OnBindableIsItemsHostChanged));

    private static void OnBindableIsItemsHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel panel)
        {
            panel.IsItemsHost = (Visibility)e.NewValue == Visibility.Visible;
        }
    }
}
