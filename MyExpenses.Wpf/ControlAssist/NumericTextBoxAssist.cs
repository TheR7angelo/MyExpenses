using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// ReSharper disable HeapView.ObjectAllocation.Possible
// ReSharper disable HeapView.BoxingAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable UnusedMember.Global

namespace MyExpenses.Wpf.ControlAssist;

/// <summary>
/// Provides attached properties and behaviors for enhancing TextBox controls with numeric input features.
/// This class allows developers to enforce numeric input constraints, including decimal and sign configuration,
/// rounding modes, and value range validations.
/// </summary>
/// <remarks>
/// This class is designed as a utility to assist with numeric input constraints in WPF applications.
/// The attached properties can be applied to TextBox controls to ensure various numeric behaviors.
/// </remarks>
public static class NumericTextBoxAssist
{
    /// <summary>
    /// Represents an attached property that specifies whether the associated TextBox
    /// should restrict input to numeric values only.
    /// </summary>
    /// <remarks>
    /// This property is typically used to enforce numeric input behavior for TextBox controls.
    /// It is a DependencyProperty and can be set in XAML or programmatically.
    /// </remarks>
    public static readonly DependencyProperty IsNumericOnlyProperty =
        DependencyProperty.RegisterAttached(
            "IsNumericOnly",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(false, OnIsNumericOnlyChanged));

    /// <summary>
    /// Represents an attached property that specifies whether the associated TextBox
    /// should allow input of decimal values.
    /// </summary>
    /// <remarks>
    /// This property is typically used in conjunction with numeric input behavior
    /// to enable or disable the use of decimal points in input values.
    /// It is a DependencyProperty and can be set in XAML or programmatically.
    /// </remarks>
    public static readonly DependencyProperty AllowDecimalProperty =
        DependencyProperty.RegisterAttached(
            "AllowDecimal",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    /// <summary>
    /// Represents an attached property that specifies whether the associated TextBox
    /// should allow negative numeric values as input.
    /// </summary>
    /// <remarks>
    /// This property is typically used to control whether users can input negative numbers
    /// in a TextBox. It is a DependencyProperty and can be configured in XAML or programmatically
    /// to enable or restrict negative values as required.
    /// </remarks>
    public static readonly DependencyProperty AllowNegativeProperty =
        DependencyProperty.RegisterAttached(
            "AllowNegative",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    /// <summary>
    /// Represents an attached property that specifies whether the associated TextBox
    /// should allow positive numeric input values.
    /// </summary>
    /// <remarks>
    /// This property is typically used to control if positive numbers can be entered
    /// or processed in conjunction with other numeric input constraints.
    /// It is a DependencyProperty and can be set in XAML or programmatically.
    /// </remarks>
    public static readonly DependencyProperty AllowPositiveProperty =
        DependencyProperty.RegisterAttached(
            "AllowPositive",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    /// <summary>
    /// Represents an attached property that specifies the minimum allowable value for input
    /// in a TextBox or similar control when numeric input behavior is enforced.
    /// </summary>
    /// <remarks>
    /// This property is typically used in conjunction with other numeric input-related properties
    /// to define validation boundaries for user input. It accepts a nullable double value, allowing
    /// flexibility for cases where no minimum value is required. It is a DependencyProperty and can
    /// be set in XAML or programmatically.
    /// </remarks>
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.RegisterAttached(
            "MinValue",
            typeof(double?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    /// <summary>
    /// Represents an attached property that defines the maximum allowable numeric value
    /// for the associated control.
    /// </summary>
    /// <remarks>
    /// This property is typically used to enforce an upper limit for numeric input in controls.
    /// It supports nullable double values and can be used in XAML or programmatically.
    /// Controls using this property should validate input to ensure it does not exceed the specified maximum value.
    /// </remarks>
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.RegisterAttached(
            "MaxValue",
            typeof(double?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    /// <summary>
    /// Represents an attached property that specifies the maximum allowable number of digits
    /// after the decimal point for numeric input in the associated control.
    /// </summary>
    /// <remarks>
    /// This property is used to enforce precision by limiting the number of decimal places
    /// a user can enter. It is a nullable integer type and can be set programmatically or through XAML.
    /// </remarks>
    public static readonly DependencyProperty MaxDecimalProperty =
        DependencyProperty.RegisterAttached(
            "MaxDecimal",
            typeof(int?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    /// <summary>
    /// Represents an attached property that specifies the rounding mode to be used when handling numeric input.
    /// </summary>
    /// <remarks>
    /// This property determines how numeric values are rounded in scenarios where rounding is necessary,
    /// such as when working with floating-point inputs or outputs.
    /// It is a DependencyProperty that can be set to a value from the <see cref="MidpointRounding"/> enumeration,
    /// allowing control over the rounding behavior (e.g., AwayFromZero, ToEven).
    /// </remarks>
    public static readonly DependencyProperty RoundingModeProperty =
        DependencyProperty.RegisterAttached(
            "RoundingMode",
            typeof(MidpointRounding),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(MidpointRounding.AwayFromZero));

    /// <summary>
    /// Gets the value of the IsNumericOnly attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the IsNumericOnly property value.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the IsNumericOnly property is set to true for the specified object.
    /// </returns>
    public static bool GetIsNumericOnly(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsNumericOnlyProperty);
    }

    /// <summary>
    /// Sets the value of the IsNumericOnly attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the IsNumericOnly property value.
    /// </param>
    /// <param name="value">
    /// A boolean value indicating whether the object should enforce numeric-only input.
    /// </param>
    public static void SetIsNumericOnly(DependencyObject obj, bool value)
    {
        obj.SetValue(IsNumericOnlyProperty, value);
    }

    /// <summary>
    /// Gets the value of the AllowDecimal attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the AllowDecimal property value.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the AllowDecimal property is set to true for the specified object.
    /// </returns>
    public static bool GetAllowDecimal(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowDecimalProperty);
    }

    /// <summary>
    /// Sets the value of the AllowDecimal attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the AllowDecimal property value.
    /// </param>
    /// <param name="value">
    /// A boolean value indicating whether decimal values are allowed for the specified object.
    /// </param>
    public static void SetAllowDecimal(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowDecimalProperty, value);
    }

    /// <summary>
    /// Gets the value of the AllowNegative attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the AllowNegative property value.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the AllowNegative property is set to true for the specified object.
    /// </returns>
    private static bool GetAllowNegative(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowNegativeProperty);
    }

    /// <summary>
    /// Sets the value of the AllowNegative attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the AllowNegative property value.
    /// </param>
    /// <param name="value">
    /// A boolean value indicating whether negative values are allowed for the specified object.
    /// </param>
    public static void SetAllowNegative(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowNegativeProperty, value);
    }

    /// <summary>
    /// Gets the value of the AllowPositive attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the AllowPositive property value.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the AllowPositive property is set to true for the specified object.
    /// </returns>
    private static bool GetAllowPositive(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowPositiveProperty);
    }

    /// <summary>
    /// Sets the value of the AllowPositive attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the AllowPositive property value.
    /// </param>
    /// <param name="value">
    /// A boolean value to set, indicating whether positive numbers are allowed.
    /// </param>
    public static void SetAllowPositive(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowPositiveProperty, value);
    }

    /// <summary>
    /// Retrieves the value of the MinValue attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the MinValue property value.
    /// </param>
    /// <returns>
    /// A nullable double that represents the minimum value constraint for the specified object.
    /// </returns>
    private static double? GetMinValue(DependencyObject obj)
    {
        return (double?)obj.GetValue(MinValueProperty);
    }

    /// <summary>
    /// Sets the minimum value allowed for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject for which to set the minimum allowable value.
    /// </param>
    /// <param name="value">
    /// A nullable double indicating the minimum allowable value to be set.
    /// </param>
    public static void SetMinValue(DependencyObject obj, double? value)
    {
        obj.SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets the value of the MaxValue attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the MaxValue property value.
    /// </param>
    /// <returns>
    /// A nullable double representing the maximum value allowed for the specified object.
    /// </returns>
    private static double? GetMaxValue(DependencyObject obj)
    {
        return (double?)obj.GetValue(MaxValueProperty);
    }

    /// <summary>
    /// Sets the value of the MaxValue attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the MaxValue property.
    /// </param>
    /// <param name="value">
    /// The maximum numeric value to set for the specified object, or null if no maximum value is defined.
    /// </param>
    public static void SetMaxValue(DependencyObject obj, double? value)
    {
        obj.SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets the value of the MaxDecimal attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the MaxDecimal property value.
    /// </param>
    /// <returns>
    /// A nullable integer representing the maximum number of decimal places allowed for numeric input, or null if no limit is set.
    /// </returns>
    public static int? GetMaxDecimal(DependencyObject obj)
    {
        return (int?)obj.GetValue(MaxDecimalProperty);
    }

    /// <summary>
    /// Sets the value of the MaxDecimal attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the MaxDecimal property value.
    /// </param>
    /// <param name="value">
    /// An integer? value specifying the maximum number of decimal places allowed for numeric input.
    /// </param>
    public static void SetMaxDecimal(DependencyObject obj, int? value)
    {
        obj.SetValue(MaxDecimalProperty, value);
    }

    /// <summary>
    /// Gets the value of the RoundingMode attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject from which to retrieve the RoundingMode property value.
    /// </param>
    /// <returns>
    /// A MidpointRounding value that indicates the rounding mode applied to numeric values.
    /// </returns>
    private static MidpointRounding GetRoundingMode(DependencyObject obj)
    {
        return (MidpointRounding)obj.GetValue(RoundingModeProperty);
    }

    /// <summary>
    /// Sets the RoundingMode attached property for the specified object.
    /// </summary>
    /// <param name="obj">
    /// The DependencyObject on which to set the RoundingMode property value.
    /// </param>
    /// <param name="value">
    /// The MidpointRounding value to set for the RoundingMode property.
    /// </param>
    public static void SetRoundingMode(DependencyObject obj, MidpointRounding value)
    {
        obj.SetValue(RoundingModeProperty, value);
    }

    /// <summary>
    /// Handles changes to the IsNumericOnly attached property, enabling or disabling numeric-only input
    /// behavior on the target TextBox based on the property value.
    /// </summary>
    /// <param name="d">
    /// The DependencyObject on which the IsNumericOnly property value has changed, expected to be a TextBox.
    /// </param>
    /// <param name="e">
    /// The event arguments containing the old and new values of the IsNumericOnly property.
    /// </param>
    private static void OnIsNumericOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        if ((bool)e.NewValue)
        {
            textBox.PreviewTextInput += TextBox_OnPreviewTextInput;
            textBox.PreviewKeyDown += TextBox_OnPreviewKeyDown;
            textBox.LostFocus += TextBox_OnLostFocus;
            DataObject.AddPastingHandler(textBox, TextBox_OnPasting);
        }
        else
        {
            textBox.PreviewTextInput -= TextBox_OnPreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_OnPreviewKeyDown;
            textBox.LostFocus -= TextBox_OnLostFocus;
            DataObject.RemovePastingHandler(textBox, TextBox_OnPasting);
        }
    }

    /// <summary>
    /// Handles the PreviewTextInput event for a TextBox to enforce numeric input constraints.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, expected to be a TextBox.
    /// </param>
    /// <param name="e">
    /// The TextCompositionEventArgs containing the input text and event data.
    /// </param>
    private static void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            e.Handled = true;
            return;
        }

        var newText = GetTextAfterInput(textBox, e.Text);
        e.Handled = !IsValidInput(textBox, newText, allowIntermediateValues: true);
    }

    /// <summary>
    /// Handles the PreviewKeyDown event for a TextBox, preventing specific key inputs according to the configured behaviors.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, expected to be a TextBox.
    /// </param>
    /// <param name="e">
    /// The KeyEventArgs containing information about the key that was pressed.
    /// </param>
    private static void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Space)
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Handles the pasting event for a TextBox to ensure that only valid numeric input is allowed based on the defined properties.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, expected to be a TextBox.
    /// </param>
    /// <param name="e">
    /// The DataObjectPastingEventArgs containing the details of the paste operation.
    /// </param>
    private static void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (sender is not TextBox textBox || !e.DataObject.GetDataPresent(DataFormats.Text) || e.DataObject.GetData(DataFormats.Text) is not string pastedText)
        {
            e.CancelCommand();
            return;
        }

        var newText = GetTextAfterInput(textBox, pastedText);

        if (!IsValidInput(textBox, newText, allowIntermediateValues: false))
        {
            e.CancelCommand();
        }
    }

    /// <summary>
    /// Handles the LostFocus event for a TextBox, ensuring that the text adheres to the numeric constraints set by the attached properties.
    /// </summary>
    /// <param name="sender">
    /// The source of the LostFocus event, expected to be a TextBox.
    /// </param>
    /// <param name="e">
    /// The event data for the LostFocus event.
    /// </param>
    private static void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            var minValue = GetMinValue(textBox);
            textBox.Text = FormatValue(textBox, minValue ?? 0);
            return;
        }

        if (IsIntermediateValue(textBox.Text) || !TryParse(textBox.Text, out var value))
        {
            var minValue = GetMinValue(textBox);
            textBox.Text = FormatValue(textBox, minValue ?? 0);
            return;
        }

        value = ClampValue(textBox, value);
        value = RoundValue(textBox, value);

        textBox.Text = FormatValue(textBox, value);
    }

    /// <summary>
    /// Calculates the resulting text of a TextBox after applying the given input,
    /// considering the current selection within the TextBox.
    /// </summary>
    /// <param name="textBox">
    /// The TextBox instance where the input is being applied.
    /// </param>
    /// <param name="input">
    /// The string input to be inserted into the TextBox at the current caret position
    /// or to replace the current selection.
    /// </param>
    /// <returns>
    /// A string representing the text of the TextBox after applying the input.
    /// </returns>
    private static string GetTextAfterInput(TextBox textBox, string input)
    {
        return textBox.Text
            .Remove(textBox.SelectionStart, textBox.SelectionLength)
            .Insert(textBox.SelectionStart, input);
    }

    /// <summary>
    /// Determines whether the specified text input is valid for the given TextBox, considering the configured
    /// constraints such as numeric formatting, value ranges, and additional validation rules.
    /// </summary>
    /// <param name="tb">
    /// The TextBox control for which the input validation is performed.
    /// </param>
    /// <param name="text">
    /// The text input that needs to be validated.
    /// </param>
    /// <param name="allowIntermediateValues">
    /// A boolean value indicating whether intermediate values (e.g., partially complete input during typing)
    /// are permissible during validation.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the text input is valid for the specified TextBox based on the current
    /// constraints.
    /// </returns>
    private static bool IsValidInput(TextBox tb, string text, bool allowIntermediateValues)
    {
        if (string.IsNullOrWhiteSpace(text)) return true;

        if (allowIntermediateValues && IsAllowedIntermediateValue(tb, text)) return true;

        var allowDec = GetAllowDecimal(tb);
        var allowNeg = GetAllowNegative(tb);
        var allowPos = GetAllowPositive(tb);
        var min = GetMinValue(tb);
        var max = GetMaxValue(tb);

        if (!TryParse(text, out var v)) return false;

        return text switch
        {
            _ when !allowDec && HasDecimalSeparator(text) => false,
            _ when !allowNeg && v < 0 => false,
            _ when !allowPos && v > 0 => false,
            _ when v < min || v > max => false,
            _ => true
        };

    }

    /// <summary>
    /// Determines whether the specified intermediate value is allowed within the provided TextBox.
    /// </summary>
    /// <param name="textBox">
    /// The TextBox control for which the intermediate value is being validated.
    /// </param>
    /// <param name="text">
    /// The intermediate value being checked, such as a partial numeric or formatting input.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the intermediate value is allowed based on the TextBox's configuration.
    /// </returns>
    private static bool IsAllowedIntermediateValue(TextBox textBox, string text)
    {
        return text switch
        {
            "." or "," => GetAllowDecimal(textBox),
            "-" => GetAllowNegative(textBox),
            "-." or "-," => GetAllowNegative(textBox) && GetAllowDecimal(textBox),
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the specified text represents an intermediate numeric value,
    /// such as a partial input during user entry (e.g., "-", ".", or "-.").
    /// </summary>
    /// <param name="text">
    /// The text to evaluate as an intermediate numeric value.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the text is considered an intermediate numeric value.
    /// </returns>
    private static bool IsIntermediateValue(string text)
    {
        return text is "-" or "." or "," or "-." or "-,";
    }

    /// <summary>
    /// Determines whether the specified text contains a decimal separator.
    /// </summary>
    /// <param name="text">
    /// The text to evaluate for the presence of a decimal separator.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the specified text contains a decimal separator, such as a period (.) or a comma (,).
    /// </returns>
    private static bool HasDecimalSeparator(string text)
    {
        return text.Contains('.') || text.Contains(',');
    }

    /// <summary>
    /// Attempts to parse the specified string as a double-precision floating-point number.
    /// </summary>
    /// <param name="text">
    /// The input string to parse, which may contain a numeric value.
    /// </param>
    /// <param name="value">
    /// When this method returns, contains the double-precision floating-point value equivalent
    /// to the numeric value in the input string, if the parsing succeeds; otherwise, contains 0.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the parsing operation was successful.
    /// </returns>
    private static bool TryParse(string text, out double value)
    {
        var normalizedText = text.Replace(',', '.');

        return double.TryParse(
            normalizedText,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
    }

    /// <summary>
    /// Clamps the specified value to be within the allowable range defined by the minimum and maximum values
    /// associated with the given TextBox.
    /// </summary>
    /// <param name="textBox">
    /// The TextBox for which the clamping operation is performed. The allowable range is determined by
    /// attached properties on this TextBox.
    /// </param>
    /// <param name="value">
    /// The numeric value to be clamped within the defined range.
    /// </param>
    /// <returns>
    /// The clamped value, which will be the minimum value if the input is below the range, the maximum value
    /// if the input is above the range, or the original value if it is within the range.
    /// </returns>
    private static double ClampValue(TextBox textBox, double value)
    {
        var minValue = GetMinValue(textBox);
        if (value < minValue)
        {
            return minValue.Value;
        }

        var maxValue = GetMaxValue(textBox);
        return value > maxValue ? maxValue.Value : value;
    }

    /// <summary>
    /// Rounds the specified value to the defined number of decimal places using the rounding mode associated with the given TextBox.
    /// </summary>
    /// <param name="textBox">
    /// The TextBox control that contains the attached properties defining the rounding behavior, such as the number of decimal places and rounding mode.
    /// </param>
    /// <param name="value">
    /// The numeric value to be rounded.
    /// </param>
    /// <returns>
    /// A double representing the rounded value based on the specified decimal places and rounding mode.
    /// </returns>
    private static double RoundValue(TextBox textBox, double value)
    {
        var maxDecimalPlaces = GetMaxDecimal(textBox);
        if (maxDecimalPlaces is null)
        {
            return value;
        }

        var decimalPlaces = Math.Max(0, maxDecimalPlaces.Value);
        var roundingMode = GetRoundingMode(textBox);

        return Math.Round(value, decimalPlaces, roundingMode);
    }

    /// <summary>
    /// Formats the specified numeric value according to the configuration of the provided TextBox, such as the maximum number of decimal places.
    /// </summary>
    /// <param name="textBox">
    /// The TextBox whose configuration (e.g., maximum decimal places) is used to format the numeric value.
    /// </param>
    /// <param name="value">
    /// The numeric value to be formatted.
    /// </param>
    /// <returns>
    /// A string representation of the formatted numeric value.
    /// </returns>
    private static string FormatValue(TextBox textBox, double value)
    {
        var maxDecimalPlaces = GetMaxDecimal(textBox);
        if (maxDecimalPlaces is null)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        var decimalPlaces = Math.Max(0, maxDecimalPlaces.Value);
        // ReSharper disable once HeapView.ObjectAllocation
        return value.ToString($"F{decimalPlaces}", CultureInfo.CurrentCulture);
    }
}