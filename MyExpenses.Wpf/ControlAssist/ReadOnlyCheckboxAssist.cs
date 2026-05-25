using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyExpenses.Wpf.ControlAssist;

/// <summary>
/// Provides attached properties and behavior to enhance the functionality of CheckBox controls.
/// Specifically, it introduces a read-only mode that disables user interactions while allowing the UI to reflect state changes.
/// </summary>
public static class ReadOnlyCheckboxAssist
{
    /// <summary>
    /// Represents an attached dependency property that specifies whether a CheckBox is in read-only mode.
    /// When set to true, the CheckBox reflects state changes visually but prevents any user interaction,
    /// including mouse clicks or keyboard input.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.RegisterAttached(
            "IsReadOnly",
            typeof(bool),
            typeof(ReadOnlyCheckboxAssist),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

    /// <summary>
    /// Gets the value of the IsReadOnly attached property for a specified DependencyObject.
    /// This indicates whether a CheckBox is in read-only mode, where the control reflects
    /// state changes visually but prevents user interaction.
    /// </summary>
    /// <param name="obj">The DependencyObject from which the IsReadOnly property value is read.</param>
    /// <returns>True if the CheckBox is in read-only mode; otherwise, false.</returns>
    public static bool GetIsReadOnly(DependencyObject obj)
        => (bool)obj.GetValue(IsReadOnlyProperty);

    /// <summary>
    /// Sets the value of the IsReadOnly attached property for a specified DependencyObject.
    /// This determines whether a CheckBox is in read-only mode, restricting user interaction
    /// while still allowing the control to reflect state changes visually.
    /// </summary>
    /// <param name="obj">The DependencyObject on which to set the IsReadOnly property value.</param>
    /// <param name="value">A boolean value indicating whether the CheckBox should be read-only (true) or interactive (false).</param>
    public static void SetIsReadOnly(DependencyObject obj, bool value)
        => obj.SetValue(IsReadOnlyProperty, value);

    /// <summary>
    /// Handles changes to the IsReadOnly attached property. This method is triggered whenever the IsReadOnly value
    /// on a DependencyObject changes, updating the behavior of the associated CheckBox to enable or disable
    /// user interactions accordingly.
    /// </summary>
    /// <param name="d">The DependencyObject on which the IsReadOnly property has been changed.</param>
    /// <param name="e">Contains event data, including the old and new values of the IsReadOnly property.</param>
    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBox checkBox) return;

        checkBox.PreviewMouseDown -= BlockClick;
        checkBox.PreviewKeyDown -= BlockKeyboard;

        if (!(bool)e.NewValue) return;

        checkBox.PreviewMouseDown += BlockClick;
        checkBox.PreviewKeyDown += BlockKeyboard;
    }

    /// <summary>
    /// Handles the PreviewMouseDown event for a CheckBox in read-only mode.
    /// Prevents the CheckBox from processing mouse click events, effectively
    /// disabling user interactions while keeping the control visually responsive.
    /// </summary>
    /// <param name="sender">The source of the event, expected to be a CheckBox.</param>
    /// <param name="e">The mouse button event data associated with the PreviewMouseDown event.</param>
    private static void BlockClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;

        e.Handled = true;
        checkBox.Focus();
    }

    /// <summary>
    /// Handles the suppression of keyboard input for a CheckBox in read-only mode. Specifically,
    /// it prevents the CheckBox from responding to the space key press, which would otherwise
    /// toggle the CheckBox's state.
    /// </summary>
    /// <param name="sender">The source of the event, typically the CheckBox.</param>
    /// <param name="e">The event data encapsulating details about the key press.</param>
    private static void BlockKeyboard(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Space) e.Handled = true;
    }
}