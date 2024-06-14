using System.Windows;
using System.Windows.Controls.Primitives;

namespace MyExpenses.Wpf.Utils.ReadOnly;

/// <summary>
/// Provides extension methods for the ToggleButton class.
/// </summary>
public static class ToggleButtonExtensions
{
    /// <summary>
    /// Provides extension methods for the ToggleButton class.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
        "IsReadOnly", typeof(bool), typeof(ToggleButtonExtensions), new PropertyMetadata(default(bool), OnPropertyChanged));

    /// <summary>
    /// The method called when the value of the attached property IsReadOnly changes.
    /// </summary>
    /// <param name="d">The dependency object to which the attached property is attached.</param>
    /// <param name="e">The event arguments containing the old and new values of the property.</param>
    /// <exception cref="InvalidOperationException">Thrown when the attached property is not set on a ToggleButton.</exception>
    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ToggleButton toggleButton)
            throw new InvalidOperationException($"This property may only be set on {nameof(ToggleButton)}.");

        if ((bool)e.NewValue)
        {
            toggleButton.Checked += OnCheckChanged;
            toggleButton.Unchecked += OnCheckChanged;
        }
        else
        {
            toggleButton.Checked -= OnCheckChanged;
            toggleButton.Unchecked -= OnCheckChanged;
        }
    }

    /// <summary>
    /// The method called when the value of the attached property IsReadOnly changes.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="e">The event arguments containing additional information about the event.</param>
    private static void OnCheckChanged(object sender, RoutedEventArgs e)
    {
        var binding = ((ToggleButton)sender).GetBindingExpression(ToggleButton.IsCheckedProperty);
        binding?.UpdateTarget();
    }

    /// <summary>
    /// Sets the value of the attached property IsReadOnly on the specified dependency object.
    /// </summary>
    /// <param name="element">The dependency object on which to set the value of the IsReadOnly attached property.</param>
    /// <param name="value">The value to set for the IsReadOnly attached property.</param>
    public static void SetIsReadOnly(DependencyObject element, bool value)
    {
        element.SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets the value of the attached property IsReadOnly on the specified dependency object.
    /// </summary>
    /// <param name="element">The dependency object from which to get the value of the IsReadOnly attached property.</param>
    /// <returns>Returns the value of the IsReadOnly attached property.</returns>
    public static bool GetIsReadOnly(DependencyObject element)
    {
        return (bool)element.GetValue(IsReadOnlyProperty);
    }
}