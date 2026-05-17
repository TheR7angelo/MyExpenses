using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MyExpenses.Wpf.Utils;

/// <summary>
/// Provides a set of attached properties and utility methods to enhance the behavior of ItemsControl.
/// </summary>
public static class ItemsControlExtensions
{
    /// <summary>
    /// Represents an attached property that determines whether the items within an
    /// <see cref="ItemsControl"/> should have uniform sizes.
    /// When set to true, this property ensures that all items in the control are assigned
    /// the maximum width required to fit the largest item, creating a uniform appearance.
    /// </summary>
    public static readonly DependencyProperty UniformSizeProperty =
        DependencyProperty.RegisterAttached("UniformSize", typeof(bool), typeof(ItemsControlExtensions),
            new PropertyMetadata(false, OnPropertiesChanged));

    /// <summary>
    /// Represents an attached property that determines whether the items within an
    /// <see cref="ItemsControl"/> should dynamically update their layout when the
    /// control's items source or collection changes.
    /// When set to true, this property ensures that the layout is recalculated
    /// in real-time, supporting scenarios where frequent updates to the control's
    /// content are necessary.
    /// </summary>
    public static readonly DependencyProperty IsLiveUpdateProperty =
        DependencyProperty.RegisterAttached("IsLiveUpdate", typeof(bool), typeof(ItemsControlExtensions),
            new PropertyMetadata(false, OnPropertiesChanged));

    /// <summary>
    /// Sets the value of the UniformSize attached property for the specified <see cref="DependencyObject"/>.
    /// This property determines whether all items within an <see cref="ItemsControl"/>
    /// should be rendered with a uniform size, where size is based on the largest item's requirements.
    /// </summary>
    /// <param name="element">The <see cref="DependencyObject"/> to which the property is attached.</param>
    /// <param name="value">A boolean value indicating whether uniform sizing is enabled (true) or disabled (false).</param>
    public static void SetUniformSize(DependencyObject element, bool value)
        => element.SetValue(UniformSizeProperty, value);

    /// <summary>
    /// Gets the value of the UniformSize attached property for the specified <see cref="DependencyObject"/>.
    /// This property determines whether all items within an <see cref="ItemsControl"/>
    /// should be displayed with a uniform size, where the size is based on the largest item's requirements.
    /// </summary>
    /// <param name="element">The <see cref="DependencyObject"/> from which the property value is read.</param>
    /// <returns>A boolean value indicating whether uniform sizing is enabled (true) or disabled (false).</returns>
    public static bool GetUniformSize(DependencyObject element)
        => (bool)element.GetValue(UniformSizeProperty);

    /// <summary>
    /// Sets the value of the IsLiveUpdate attached property for the specified <see cref="DependencyObject"/>.
    /// This property determines whether the layout of items within an <see cref="ItemsControl"/> dynamically updates
    /// in real time when the data source or collection changes, supporting scenarios that require frequent updates.
    /// </summary>
    /// <param name="element">The <see cref="DependencyObject"/> to which the property is attached.</param>
    /// <param name="value">A boolean value indicating whether live updates are enabled (true) or disabled (false).</param>
    public static void SetIsLiveUpdate(DependencyObject element, bool value)
        => element.SetValue(IsLiveUpdateProperty, value);

    /// <summary>
    /// Gets the value of the IsLiveUpdate attached property for the specified <see cref="DependencyObject"/>.
    /// This property determines whether the layout of an <see cref="ItemsControl"/> is dynamically updated
    /// in response to changes in its items source or collection.
    /// </summary>
    /// <param name="element">The <see cref="DependencyObject"/> from which to retrieve the property value.</param>
    /// <returns>A boolean value indicating whether live updates are enabled (true) or disabled (false).</returns>
    public static bool GetIsLiveUpdate(DependencyObject element)
        => (bool)element.GetValue(IsLiveUpdateProperty);

    /// <summary>
    /// Handles changes to attached properties on an <see cref="ItemsControl"/> and updates its behavior accordingly.
    /// This method is invoked when a dependency property associated with the behavior changes, such as
    /// <see cref="UniformSizeProperty"/> or <see cref="IsLiveUpdateProperty"/>.
    /// It sets up necessary event handlers for functionality like uniform sizing or live data updates.
    /// </summary>
    /// <param name="d">The <see cref="DependencyObject"/> whose property has changed. Expected to be an <see cref="ItemsControl"/>.</param>
    /// <param name="e">Provides information about the property that changed, including its old and new values.</param>
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

    /// <summary>
    /// Handles the Loaded event of an <see cref="ItemsControl"/> to apply necessary adjustments
    /// when the control is rendered. This method ensures that size adjustments are applied
    /// if the UniformSize attached property is set to true.
    /// </summary>
    /// <param name="sender">The source of the event, typically the <see cref="ItemsControl"/> that was loaded.</param>
    /// <param name="e">The event data containing information about the Loaded event.</param>
    private static void OnControlLoaded(object sender, RoutedEventArgs e)
        => UpdateSizes(sender as ItemsControl);

    /// <summary>
    /// Handles the event triggered when the <see cref="ItemsControl.ItemsSource"/> property of an <see cref="ItemsControl"/> changes.
    /// This method ensures that the underlying data collection is monitored for changes and updates the sizes of items accordingly
    /// if the uniform sizing feature is enabled.
    /// </summary>
    /// <param name="sender">The object that raised the event, expected to be an <see cref="ItemsControl"/>.</param>
    /// <param name="e">The event data associated with the change.</param>
    private static void OnItemsSourceChanged(object sender, EventArgs e)
    {
        if (sender is not ItemsControl ic) return;

        HookCollection(ic);
        UpdateSizes(ic);
    }

    /// <summary>
    /// Subscribes to the <see cref="INotifyCollectionChanged.CollectionChanged"/> event of the ItemsControl's ItemsSource
    /// to monitor changes and trigger updates to the uniform sizing logic as necessary.
    /// </summary>
    /// <param name="ic">The <see cref="ItemsControl"/> whose ItemsSource collection will be monitored.</param>
    private static void HookCollection(ItemsControl ic)
    {
        if (ic.ItemsSource is not INotifyCollectionChanged collection) return;

        collection.CollectionChanged -= (_, _) => UpdateSizes(ic);
        collection.CollectionChanged += (_, _) => UpdateSizes(ic);
    }

    /// <summary>
    /// Updates the sizes of the items within the specified <see cref="ItemsControl"/> to ensure uniform sizing
    /// if the UniformSize attached property is set to true. This method calculates the maximum required width
    /// across all items and applies it uniformly to all containers.
    /// </summary>
    /// <param name="ic">The <see cref="ItemsControl"/> whose item sizes are to be updated.</param>
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