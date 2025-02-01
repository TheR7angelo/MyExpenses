using System.Runtime.Versioning;

namespace MyExpenses.Maui.Utils.Margin
{
    /// <summary>
    /// Provides attached properties for the <see cref="Microsoft.Maui.Controls.Layout"/> class.
    /// </summary>
    public static class LayoutExtensions
    {
        /// <summary>
        /// Provides an attached property for setting the margin for all children of a <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        [SupportedOSPlatform("Android21.0")]
        [SupportedOSPlatform("iOS15.0")]
        [SupportedOSPlatform("MacCatalyst14.0")]
        [SupportedOSPlatform("Windows")]
        public static readonly BindableProperty MarginForAllChildrenProperty =
            BindableProperty.CreateAttached("MarginForAllChildren", typeof(Thickness),
                typeof(LayoutExtensions), null, propertyChanged: OnMarginForAllChildrenChanged);

        /// <summary>
        /// Provides an attached property for setting the margin for all children of a <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The Layout instance to set the margin for its children.</param>
        /// <param name="value">The margin value to be set for all children.</param>
        [SupportedOSPlatform("Android21.0")]
        [SupportedOSPlatform("iOS15.0")]
        [SupportedOSPlatform("MacCatalyst14.0")]
        [SupportedOSPlatform("Windows")]
        public static void SetMarginForAllChildren(this Layout layout, Thickness value)
            // ReSharper disable once HeapView.BoxingAllocation
            => layout.SetValue(MarginForAllChildrenProperty, value);

        /// <summary>
        /// Gets the margin value set for all children of a <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The Layout instance to get the margin for its children.</param>
        /// <returns>The margin value set for all children.</returns>
        [SupportedOSPlatform("Android21.0")]
        [SupportedOSPlatform("iOS15.0")]
        [SupportedOSPlatform("MacCatalyst14.0")]
        [SupportedOSPlatform("Windows")]
        public static Thickness GetMarginForAllChildren(this Layout layout) =>
            (Thickness)layout.GetValue(MarginForAllChildrenProperty);

        /// <summary>
        /// Handles changes to the MarginForAllChildren attached property and updates the margin of all child elements accordingly.
        /// </summary>
        /// <param name="bindable">The object to which the property is attached, expected to be of type <see cref="Microsoft.Maui.Controls.Layout"/>.</param>
        /// <param name="oldValue">The old value of the MarginForAllChildren property.</param>
        /// <param name="newValue">The new value of the MarginForAllChildren property.</param>
        [SupportedOSPlatform("Android21.0")]
        [SupportedOSPlatform("iOS15.0")]
        [SupportedOSPlatform("MacCatalyst14.0")]
        [SupportedOSPlatform("Windows")]
        private static void OnMarginForAllChildrenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Layout layout) return;
            var margin = (Thickness)newValue;

            ApplyMarginToAllChildren(layout, margin);

            layout.ChildAdded += (_, e) => ApplyMarginToChild(e.Element as View, margin);
        }

        /// <summary>
        /// Applies the specified margin to all children of the given <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The Layout instance whose children's margins will be set.</param>
        /// <param name="margin">The margin value to be applied to all children of the layout.</param>
        private static void ApplyMarginToAllChildren(Layout layout, Thickness margin)
        {
            foreach (var child in layout.Children)
            {
                ApplyMarginToChild(child as View, margin);
            }
        }

        /// <summary>
        /// Applies the specified margin to a child view if its current margin is not set.
        /// </summary>
        /// <param name="child">The child view to which the margin will be applied.</param>
        /// <param name="margin">The margin value to be applied to the child view.</param>
        private static void ApplyMarginToChild(View? child, Thickness margin)
        {
            if (child != null && child.Margin == default)
            {
                child.Margin = margin;
            }
        }
    }
}