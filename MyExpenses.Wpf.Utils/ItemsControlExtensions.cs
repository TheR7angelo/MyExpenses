using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Utils;

public static class ItemsControlExtensions
{
    public static readonly DependencyProperty UniformSizeProperty =
        DependencyProperty.RegisterAttached("UniformSize", typeof(bool), typeof(ItemsControlExtensions),
            new PropertyMetadata(false, OnPropertiesChanged));

    public static readonly DependencyProperty IsLiveUpdateProperty =
        DependencyProperty.RegisterAttached("IsLiveUpdate", typeof(bool), typeof(ItemsControlExtensions),
            new PropertyMetadata(false, OnPropertiesChanged));

    public static void SetUniformSize(DependencyObject element, bool value) => element.SetValue(UniformSizeProperty, value);
    public static bool GetUniformSize(DependencyObject element) => (bool)element.GetValue(UniformSizeProperty);

    public static void SetIsLiveUpdate(DependencyObject element, bool value) => element.SetValue(IsLiveUpdateProperty, value);
    public static bool GetIsLiveUpdate(DependencyObject element) => (bool)element.GetValue(IsLiveUpdateProperty);

    private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ItemsControl ic) return;

        ic.Loaded -= OnControlLoaded;

        if (!GetUniformSize(ic)) return;
        ic.Loaded += OnControlLoaded;

        if (!GetIsLiveUpdate(ic)) return;

        var descriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl));
        descriptor.RemoveValueChanged(ic, OnItemsSourceChanged);
        descriptor.AddValueChanged(ic, OnItemsSourceChanged);

        HookCollection(ic);
    }

    private static void OnControlLoaded(object sender, RoutedEventArgs e)
        => UpdateSizes(sender as ItemsControl);

    private static void OnItemsSourceChanged(object sender, EventArgs e)
    {
        if (sender is not ItemsControl ic) return;

        HookCollection(ic);
        UpdateSizes(ic);
    }

    private static void HookCollection(ItemsControl ic)
    {
        if (ic.ItemsSource is not INotifyCollectionChanged collection) return;

        collection.CollectionChanged -= (_, _) => UpdateSizes(ic);
        collection.CollectionChanged += (_, _) => UpdateSizes(ic);
    }

    private static void UpdateSizes(ItemsControl ic)
    {
        if (ic is not { IsLoaded: true } || !GetUniformSize(ic)) return;

        ic.Dispatcher.BeginInvoke(new Action(() =>
        {
            double maxWidth = 0;
            var containers = new List<FrameworkElement>();

            foreach (var item in ic.Items)
            {
                if (ic.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container) continue;

                container.Width = double.NaN;
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                maxWidth = Math.Max(maxWidth, container.DesiredSize.Width);
                containers.Add(container);
            }

            foreach (var c in containers) c.Width = maxWidth;

        }), System.Windows.Threading.DispatcherPriority.Render);
    }
}