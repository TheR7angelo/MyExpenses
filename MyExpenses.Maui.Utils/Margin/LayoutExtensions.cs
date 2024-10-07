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
        public static readonly BindableProperty MarginForAllChildrenProperty =
            BindableProperty.CreateAttached("MarginForAllChildren", typeof(Thickness),
                typeof(LayoutExtensions), default(Thickness), propertyChanged: OnMarginForAllChildrenChanged);

        /// <summary>
        /// Provides an attached property for setting the margin for all children of a <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The Layout instance to set the margin for its children.</param>
        /// <param name="value">The margin value to be set for all children.</param>
        public static void SetMarginForAllChildren(this Layout layout, Thickness value) =>
            layout.SetValue(MarginForAllChildrenProperty, value);

        /// <summary>
        /// Gets the margin value set for all children of a <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The Layout instance to get the margin for its children.</param>
        /// <returns>The margin value set for all children.</returns>
        public static Thickness GetMarginForAllChildren(this Layout layout) =>
            (Thickness)layout.GetValue(MarginForAllChildrenProperty);

        /// <summary>
        /// Handler for the <see cref="LayoutExtensions.MarginForAllChildrenProperty"/> property changed event.
        /// Applies the margin value to all children of the layout.
        /// </summary>
        /// <param name="bindable">The bindable object representing the layout.</param>
        /// <param name="oldValue">The old margin value.</param>
        /// <param name="newValue">The new margin value.</param>
        private static void OnMarginForAllChildrenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Layout layout)
            {
                ApplyMargin(layout, (Thickness)newValue);
                layout.ChildAdded += (_, _) => ApplyMargin(layout, (Thickness)newValue);
            }
        }

        /// <summary>
        /// Applies the specified margin to all children of the specified <see cref="Microsoft.Maui.Controls.Layout"/>.
        /// </summary>
        /// <param name="layout">The <see cref="Microsoft.Maui.Controls.Layout"/> instance to apply the margin to its children.</param>
        /// <param name="margin">The margin value to be applied to all children.</param>
        private static void ApplyMargin(Layout layout, Thickness margin)
        {
            foreach (var child in layout.Children)
            {
                if (child is View view) // Assuming all children are of type View, which derives from VisualElement
                {
                    if (view.Margin == default)
                    {
                        view.Margin = margin;
                    }
                }
            }
        }
    }
}