using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

    public static bool GetAllowNegative(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowNegativeProperty);
    }

    public static void SetAllowNegative(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowNegativeProperty, value);
    }

    public static bool GetAllowPositive(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowPositiveProperty);
    }

    public static void SetAllowPositive(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowPositiveProperty, value);
    }

    public static double? GetMinValue(DependencyObject obj)
    {
        return (double?)obj.GetValue(MinValueProperty);
    }

    public static void SetMinValue(DependencyObject obj, double? value)
    {
        obj.SetValue(MinValueProperty, value);
    }

    public static double? GetMaxValue(DependencyObject obj)
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

    public static MidpointRounding GetRoundingMode(DependencyObject obj)
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
        if (sender is not TextBox textBox)
        {
            e.CancelCommand();
            return;
        }

        if (!e.DataObject.GetDataPresent(DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        var pastedText = e.DataObject.GetData(DataFormats.Text) as string;
        if (pastedText is null)
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

        if (IsIntermediateValue(textBox.Text))
        {
            textBox.Text = string.Empty;
            return;
        }

        if (!TryParse(textBox.Text, out var value))
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

    private static bool IsValidInput(TextBox textBox, string text, bool allowIntermediateValues)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (allowIntermediateValues && IsAllowedIntermediateValue(textBox, text))
        {
            return true;
        }

        if (!GetAllowDecimal(textBox) && HasDecimalSeparator(text))
        {
            return false;
        }

        if (!TryParse(text, out var value))
        {
            return false;
        }

        if (!GetAllowNegative(textBox) && value < 0)
        {
            return false;
        }

        if (!GetAllowPositive(textBox) && value > 0)
        {
            return false;
        }

        var minValue = GetMinValue(textBox);
        if (minValue is not null && value < minValue.Value)
        {
            return false;
        }

        var maxValue = GetMaxValue(textBox);
        if (maxValue is not null && value > maxValue.Value)
        {
            return false;
        }

        return true;
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
        if (minValue is not null && value < minValue.Value)
        {
            return minValue.Value;
        }

        var maxValue = GetMaxValue(textBox);
        if (maxValue is not null && value > maxValue.Value)
        {
            return maxValue.Value;
        }

        return value;
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
        return value.ToString($"F{decimalPlaces}", CultureInfo.CurrentCulture);
    }
}