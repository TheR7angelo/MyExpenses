using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
// ReSharper disable HeapView.ObjectAllocation.Possible
// ReSharper disable HeapView.BoxingAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable UnusedMember.Global

namespace MyExpenses.Wpf.Behaviors;

public static class NumericTextBoxAssist
{
    public static readonly DependencyProperty IsNumericOnlyProperty =
        DependencyProperty.RegisterAttached(
            "IsNumericOnly",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(false, OnIsNumericOnlyChanged));

    public static readonly DependencyProperty AllowDecimalProperty =
        DependencyProperty.RegisterAttached(
            "AllowDecimal",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    public static readonly DependencyProperty AllowNegativeProperty =
        DependencyProperty.RegisterAttached(
            "AllowNegative",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    public static readonly DependencyProperty AllowPositiveProperty =
        DependencyProperty.RegisterAttached(
            "AllowPositive",
            typeof(bool),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(true));

    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.RegisterAttached(
            "MinValue",
            typeof(double?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.RegisterAttached(
            "MaxValue",
            typeof(double?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    public static readonly DependencyProperty MaxDecimalProperty =
        DependencyProperty.RegisterAttached(
            "MaxDecimal",
            typeof(int?),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(null));

    public static readonly DependencyProperty RoundingModeProperty =
        DependencyProperty.RegisterAttached(
            "RoundingMode",
            typeof(MidpointRounding),
            typeof(NumericTextBoxAssist),
            new PropertyMetadata(MidpointRounding.AwayFromZero));

    public static bool GetIsNumericOnly(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsNumericOnlyProperty);
    }

    public static void SetIsNumericOnly(DependencyObject obj, bool value)
    {
        obj.SetValue(IsNumericOnlyProperty, value);
    }

    public static bool GetAllowDecimal(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowDecimalProperty);
    }

    public static void SetAllowDecimal(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowDecimalProperty, value);
    }

    private static bool GetAllowNegative(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowNegativeProperty);
    }

    public static void SetAllowNegative(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowNegativeProperty, value);
    }

    private static bool GetAllowPositive(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowPositiveProperty);
    }

    public static void SetAllowPositive(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowPositiveProperty, value);
    }

    private static double? GetMinValue(DependencyObject obj)
    {
        return (double?)obj.GetValue(MinValueProperty);
    }

    public static void SetMinValue(DependencyObject obj, double? value)
    {
        obj.SetValue(MinValueProperty, value);
    }

    private static double? GetMaxValue(DependencyObject obj)
    {
        return (double?)obj.GetValue(MaxValueProperty);
    }

    public static void SetMaxValue(DependencyObject obj, double? value)
    {
        obj.SetValue(MaxValueProperty, value);
    }

    public static int? GetMaxDecimal(DependencyObject obj)
    {
        return (int?)obj.GetValue(MaxDecimalProperty);
    }

    public static void SetMaxDecimal(DependencyObject obj, int? value)
    {
        obj.SetValue(MaxDecimalProperty, value);
    }

    private static MidpointRounding GetRoundingMode(DependencyObject obj)
    {
        return (MidpointRounding)obj.GetValue(RoundingModeProperty);
    }

    public static void SetRoundingMode(DependencyObject obj, MidpointRounding value)
    {
        obj.SetValue(RoundingModeProperty, value);
    }

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

    private static void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Space)
        {
            e.Handled = true;
        }
    }

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

    private static void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            return;
        }

        if (IsIntermediateValue(textBox.Text) || !TryParse(textBox.Text, out var value))
        {
            textBox.Text = string.Empty;
            return;
        }

        value = ClampValue(textBox, value);
        value = RoundValue(textBox, value);

        textBox.Text = FormatValue(textBox, value);
    }

    private static string GetTextAfterInput(TextBox textBox, string input)
    {
        return textBox.Text
            .Remove(textBox.SelectionStart, textBox.SelectionLength)
            .Insert(textBox.SelectionStart, input);
    }

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

    private static bool IsIntermediateValue(string text)
    {
        return text is "-" or "." or "," or "-." or "-,";
    }

    private static bool HasDecimalSeparator(string text)
    {
        return text.Contains('.') || text.Contains(',');
    }

    private static bool TryParse(string text, out double value)
    {
        var normalizedText = text.Replace(',', '.');

        return double.TryParse(
            normalizedText,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
    }

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