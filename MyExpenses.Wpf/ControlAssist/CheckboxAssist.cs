using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyExpenses.Wpf.ControlAssist;

/// <summary>
/// Specifies the alignment of content within a CheckBox control.
/// </summary>
public enum CheckBoxContentAlignment
{
    Right,
    Left
}

/// <summary>
/// Provides attached properties for controlling the behavior and appearance of CheckBox controls.
/// </summary>
public static class CheckboxAssist
{
    #region Attached Property - ContentAlignment

    /// <summary>
    /// Identifies the ContentAlignment attached property. This dependency property indicates the alignment of content within a CheckBox control.
    /// </summary>
    public static readonly DependencyProperty ContentAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "ContentAlignment",
            typeof(CheckBoxContentAlignment),
            typeof(CheckboxAssist),
            new PropertyMetadata(CheckBoxContentAlignment.Right, OnContentAlignmentChanged));

    /// <summary>
    /// Sets the alignment of content within a CheckBox control.
    /// </summary>
    /// <param name="element">The element to which the attached property is being set.</param>
    /// <param name="value">The value to set for the ContentAlignment attached property.</param>
    public static void SetContentAlignment(DependencyObject element, CheckBoxContentAlignment value)
        => element.SetValue(ContentAlignmentProperty, value);

    /// <summary>
    /// Retrieves the alignment of content within a CheckBox control.
    /// </summary>
    /// <param name="element">The element from which the attached property is being retrieved.</param>
    /// <returns>The value of the ContentAlignment attached property.</returns>
    public static CheckBoxContentAlignment GetContentAlignment(DependencyObject element)
        => (CheckBoxContentAlignment)element.GetValue(ContentAlignmentProperty);

    /// <summary>
    /// Handles changes to the ContentAlignment attached property.
    /// </summary>
    /// <param name="d">The dependency object with the attached property changed.</param>
    /// <param name="e">Information about the property change.</param>
    private static void OnContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBox cb || e.NewValue is not CheckBoxContentAlignment.Left) return;

        if (cb.IsLoaded)
        {
            InvertColumns(cb);
            return;
        }

        cb.Loaded -= OnCheckBoxLoaded;
        cb.Loaded += OnCheckBoxLoaded;
    }

    /// <summary>
    /// Handles the Loaded event of a CheckBox control.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The RoutedEventArgs instance containing the event data.</param>
    private static void OnCheckBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb) return;

        cb.Loaded -= OnCheckBoxLoaded;
        InvertColumns(cb);
    }

    /// <summary>
    /// Inverts the column order of a CheckBox control.
    /// </summary>
    /// <param name="cb">The CheckBox whose columns are to be inverted.</param>
    private static void InvertColumns(CheckBox cb)
    {
        cb.ApplyTemplate();

        if (cb.Template.FindName("templateRoot", cb) is not Grid targetGrid)
        {
            if (VisualTreeHelper.GetChildrenCount(cb) == 0 ||
                VisualTreeHelper.GetChild(cb, 0) is not Grid gridRoot)
            {
                return;
            }
            targetGrid = gridRoot;
        }

        SwapColumnDefinitions(targetGrid);
        SwapChildrenColumns(targetGrid);
    }

    /// <summary>
    /// Swaps the widths of the first two column definitions in a Grid.
    /// </summary>
    /// <param name="grid">The Grid containing the columns to be swapped.</param>
    private static void SwapColumnDefinitions(Grid grid)
    {
        if (grid.ColumnDefinitions.Count < 2) return;

        (grid.ColumnDefinitions[0].Width, grid.ColumnDefinitions[1].Width) = (grid.ColumnDefinitions[1].Width, grid.ColumnDefinitions[0].Width);
    }

    /// <summary>
    /// Swaps the columns of child elements in a specified grid.
    /// </summary>
    /// <param name="grid">The Grid whose children's columns will be swapped.</param>
    private static void SwapChildrenColumns(Grid grid)
    {
        foreach (UIElement child in grid.Children)
        {
            var currentColumn = Grid.GetColumn(child);
            switch (currentColumn)
            {
                case 0:
                    Grid.SetColumn(child, 1);
                    ApplyMargin(child, new Thickness(0, 0, 8, 0));
                    break;
                case 1:
                    Grid.SetColumn(child, 0);
                    ApplyMargin(child, new Thickness(0));
                    break;
            }
        }
    }

    /// <summary>
    /// Applies a margin to the specified UIElement.
    /// </summary>
    /// <param name="element">The UIElement to which the margin will be applied.</param>
    /// <param name="margin">The margin values to set for the element.</param>
    private static void ApplyMargin(UIElement element, Thickness margin)
    {
        if (element is FrameworkElement fe)
        {
            fe.Margin = margin;
        }
    }

    #endregion

    #region Attached Property - IsReadOnly

    /// <summary>
    /// Identifies the IsReadOnly attached property. This dependency property indicates whether a CheckBox control is read-only.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.RegisterAttached(
            "IsReadOnly",
            typeof(bool),
            typeof(CheckboxAssist),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

    /// <summary>
    /// Gets whether a CheckBox control is read-only.
    /// </summary>
    /// <param name="obj">The object to which the attached property is being queried.</param>
    /// <returns>true if the CheckBox control is read-only; otherwise, false.</returns>
    public static bool GetIsReadOnly(DependencyObject obj)
        => (bool)obj.GetValue(IsReadOnlyProperty);

    /// <summary>
    /// Sets whether a CheckBox control is read-only.
    /// </summary>
    /// <param name="obj">The element to which the attached property is being set.</param>
    /// <param name="value">The value to set for the IsReadOnly attached property. If true, the CheckBox is read-only; otherwise, it is editable.</param>
    public static void SetIsReadOnly(DependencyObject obj, bool value)
        => obj.SetValue(IsReadOnlyProperty, value);

    /// <summary>
    /// Handles the change of the IsReadOnly attached property.
    /// </summary>
    /// <param name="d">The dependency object where the property change occurred.</param>
    /// <param name="e">Event data containing information about the property change.</param>
    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBox checkBox) return;

        checkBox.PreviewMouseLeftButtonDown -= BlockClickAndExecuteCommand;
        checkBox.PreviewKeyDown -= BlockKeyboard;

        if (e.NewValue is not true) return;

        checkBox.PreviewMouseLeftButtonDown += BlockClickAndExecuteCommand;
        checkBox.PreviewKeyDown += BlockKeyboard;
    }

    /// <summary>
    /// Prevents a CheckBox from executing its command when clicked or key pressed.
    /// </summary>
    /// <param name="sender">The CheckBox element.</param>
    /// <param name="e">Mouse button event arguments.</param>
    private static void BlockClickAndExecuteCommand(object sender, MouseButtonEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;

        checkBox.Focus();
        ExecuteCheckBoxCommand(checkBox);
        e.Handled = true;
    }

    /// <summary>
    /// Blocks keyboard input for a CheckBox control.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event data.</param>
    private static void BlockKeyboard(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Space || sender is not CheckBox checkBox) return;

        ExecuteCheckBoxCommand(checkBox);
        e.Handled = true;
    }

    /// <summary>
    /// Executes the command associated with a CheckBox control.
    /// </summary>
    /// <param name="checkBox">The CheckBox control for which to execute the command.</param>
    private static void ExecuteCheckBoxCommand(CheckBox checkBox)
    {
        if (checkBox.Command != null && checkBox.Command.CanExecute(checkBox.CommandParameter))
        {
            checkBox.Command.Execute(checkBox.CommandParameter);
        }
    }

    #endregion
}