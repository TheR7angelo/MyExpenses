using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.SharedUtils.RegexUtils;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.UserControls.Colors;

public sealed partial class ColorPickerControl
{
    private bool _isSynchronizing;

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(ColorPickerControl),
            new FrameworkPropertyMetadata(
                System.Windows.Media.Colors.White,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                PropertyColor_OnChangedCallback));

    public static readonly DependencyProperty RedValueProperty =
        DependencyProperty.Register(
            nameof(RedValue),
            typeof(byte),
            typeof(ColorPickerControl),
            new PropertyMetadata((byte)255, PropertyRedValue_OnChangedCallback));

    public static readonly DependencyProperty GreenValueProperty =
        DependencyProperty.Register(
            nameof(GreenValue),
            typeof(byte),
            typeof(ColorPickerControl),
            new PropertyMetadata((byte)255, PropertyGreenValue_OnChangedCallback));

    public static readonly DependencyProperty BlueValueProperty =
        DependencyProperty.Register(
            nameof(BlueValue),
            typeof(byte),
            typeof(ColorPickerControl),
            new PropertyMetadata((byte)255, PropertyBlueValue_OnChangedCallback));

    public static readonly DependencyProperty AlphaValueProperty =
        DependencyProperty.Register(
            nameof(AlphaValue),
            typeof(byte),
            typeof(ColorPickerControl),
            new PropertyMetadata((byte)255, PropertyAlphaValue_OnChangedCallback));

    public static readonly DependencyProperty HueValueProperty =
        DependencyProperty.Register(
            nameof(HueValue),
            typeof(double),
            typeof(ColorPickerControl),
            new PropertyMetadata(0d, PropertyHueValue_OnChangedCallback));

    public static readonly DependencyProperty SaturationValueProperty =
        DependencyProperty.Register(
            nameof(SaturationValue),
            typeof(double),
            typeof(ColorPickerControl),
            new PropertyMetadata(0d, PropertySaturationValue_OnChangedCallback));

    public static readonly DependencyProperty ValueValueProperty =
        DependencyProperty.Register(
            nameof(ValueValue),
            typeof(double),
            typeof(ColorPickerControl),
            new PropertyMetadata(1d, PropertyValueValue_OnChangedCallback));

    public static readonly DependencyProperty RedSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(RedSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty GreenSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(GreenSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty BlueSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(BlueSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty HueSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(HueSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty SaturationSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(SaturationSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty ValueSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(ValueSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty AlphaSliderBorderThicknessProperty =
        DependencyProperty.Register(
            nameof(AlphaSliderBorderThickness),
            typeof(Thickness),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty RedSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(RedSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty GreenSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(GreenSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BlueSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(BlueSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty HueSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(HueSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty SaturationSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(SaturationSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ValueSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(ValueSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty AlphaSliderBorderBrushProperty =
        DependencyProperty.Register(
            nameof(AlphaSliderBorderBrush),
            typeof(Brush),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty LabelRedChannelProperty =
        DependencyProperty.Register(
            nameof(LabelRedChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelGreenChannelProperty =
        DependencyProperty.Register(
            nameof(LabelGreenChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelBlueChannelProperty =
        DependencyProperty.Register(
            nameof(LabelBlueChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelHueChannelProperty =
        DependencyProperty.Register(
            nameof(LabelHueChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelSaturationChannelProperty =
        DependencyProperty.Register(
            nameof(LabelSaturationChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelValueChannelProperty =
        DependencyProperty.Register(
            nameof(LabelValueChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelAlphaChannelProperty =
        DependencyProperty.Register(
            nameof(LabelAlphaChannel),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelPreviewProperty =
        DependencyProperty.Register(
            nameof(LabelPreview),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty LabelHexadecimalCodeProperty =
        DependencyProperty.Register(
            nameof(LabelHexadecimalCode),
            typeof(string),
            typeof(ColorPickerControl),
            new PropertyMetadata(default(string)));

    public ColorPickerControl()
    {
        UpdateLanguage();

        InitializeComponent();

        Loaded += ColorPickerControl_OnLoaded;

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public byte RedValue
    {
        get => (byte)GetValue(RedValueProperty);
        set => SetValue(RedValueProperty, value);
    }

    public byte GreenValue
    {
        get => (byte)GetValue(GreenValueProperty);
        set => SetValue(GreenValueProperty, value);
    }

    public byte BlueValue
    {
        get => (byte)GetValue(BlueValueProperty);
        set => SetValue(BlueValueProperty, value);
    }

    public byte AlphaValue
    {
        get => (byte)GetValue(AlphaValueProperty);
        set => SetValue(AlphaValueProperty, value);
    }

    public double HueValue
    {
        get => (double)GetValue(HueValueProperty);
        set => SetValue(HueValueProperty, value);
    }

    public double SaturationValue
    {
        get => (double)GetValue(SaturationValueProperty);
        set => SetValue(SaturationValueProperty, value);
    }

    public double ValueValue
    {
        get => (double)GetValue(ValueValueProperty);
        set => SetValue(ValueValueProperty, value);
    }

    public Thickness RedSliderBorderThickness
    {
        get => (Thickness)GetValue(RedSliderBorderThicknessProperty);
        set => SetValue(RedSliderBorderThicknessProperty, value);
    }

    public Thickness GreenSliderBorderThickness
    {
        get => (Thickness)GetValue(GreenSliderBorderThicknessProperty);
        set => SetValue(GreenSliderBorderThicknessProperty, value);
    }

    public Thickness BlueSliderBorderThickness
    {
        get => (Thickness)GetValue(BlueSliderBorderThicknessProperty);
        set => SetValue(BlueSliderBorderThicknessProperty, value);
    }

    public Thickness HueSliderBorderThickness
    {
        get => (Thickness)GetValue(HueSliderBorderThicknessProperty);
        set => SetValue(HueSliderBorderThicknessProperty, value);
    }

    public Thickness SaturationSliderBorderThickness
    {
        get => (Thickness)GetValue(SaturationSliderBorderThicknessProperty);
        set => SetValue(SaturationSliderBorderThicknessProperty, value);
    }

    public Thickness ValueSliderBorderThickness
    {
        get => (Thickness)GetValue(ValueSliderBorderThicknessProperty);
        set => SetValue(ValueSliderBorderThicknessProperty, value);
    }

    public Thickness AlphaSliderBorderThickness
    {
        get => (Thickness)GetValue(AlphaSliderBorderThicknessProperty);
        set => SetValue(AlphaSliderBorderThicknessProperty, value);
    }

    public Brush RedSliderBorderBrush
    {
        get => (Brush)GetValue(RedSliderBorderBrushProperty);
        set => SetValue(RedSliderBorderBrushProperty, value);
    }

    public Brush GreenSliderBorderBrush
    {
        get => (Brush)GetValue(GreenSliderBorderBrushProperty);
        set => SetValue(GreenSliderBorderBrushProperty, value);
    }

    public Brush BlueSliderBorderBrush
    {
        get => (Brush)GetValue(BlueSliderBorderBrushProperty);
        set => SetValue(BlueSliderBorderBrushProperty, value);
    }

    public Brush HueSliderBorderBrush
    {
        get => (Brush)GetValue(HueSliderBorderBrushProperty);
        set => SetValue(HueSliderBorderBrushProperty, value);
    }

    public Brush SaturationSliderBorderBrush
    {
        get => (Brush)GetValue(SaturationSliderBorderBrushProperty);
        set => SetValue(SaturationSliderBorderBrushProperty, value);
    }

    public Brush ValueSliderBorderBrush
    {
        get => (Brush)GetValue(ValueSliderBorderBrushProperty);
        set => SetValue(ValueSliderBorderBrushProperty, value);
    }

    public Brush AlphaSliderBorderBrush
    {
        get => (Brush)GetValue(AlphaSliderBorderBrushProperty);
        set => SetValue(AlphaSliderBorderBrushProperty, value);
    }

    public string LabelRedChannel
    {
        get => (string)GetValue(LabelRedChannelProperty);
        set => SetValue(LabelRedChannelProperty, value);
    }

    public string LabelGreenChannel
    {
        get => (string)GetValue(LabelGreenChannelProperty);
        set => SetValue(LabelGreenChannelProperty, value);
    }

    public string LabelBlueChannel
    {
        get => (string)GetValue(LabelBlueChannelProperty);
        set => SetValue(LabelBlueChannelProperty, value);
    }

    public string LabelHueChannel
    {
        get => (string)GetValue(LabelHueChannelProperty);
        set => SetValue(LabelHueChannelProperty, value);
    }

    public string LabelSaturationChannel
    {
        get => (string)GetValue(LabelSaturationChannelProperty);
        set => SetValue(LabelSaturationChannelProperty, value);
    }

    public string LabelValueChannel
    {
        get => (string)GetValue(LabelValueChannelProperty);
        set => SetValue(LabelValueChannelProperty, value);
    }

    public string LabelAlphaChannel
    {
        get => (string)GetValue(LabelAlphaChannelProperty);
        set => SetValue(LabelAlphaChannelProperty, value);
    }

    public string LabelPreview
    {
        get => (string)GetValue(LabelPreviewProperty);
        set => SetValue(LabelPreviewProperty, value);
    }

    public string LabelHexadecimalCode
    {
        get => (string)GetValue(LabelHexadecimalCodeProperty);
        set => SetValue(LabelHexadecimalCodeProperty, value);
    }

    private static void PropertyColor_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        var color = (Color)e.NewValue;
        control.SynchronizeFromColor(color);
    }

    private static void PropertyRedValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        var color = Color.FromArgb(
            control.Color.A,
            (byte)e.NewValue,
            control.Color.G,
            control.Color.B);

        control.SetColorFromComponent(color);
    }

    private static void PropertyGreenValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        var color = Color.FromArgb(
            control.Color.A,
            control.Color.R,
            (byte)e.NewValue,
            control.Color.B);

        control.SetColorFromComponent(color);
    }

    private static void PropertyBlueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        var color = Color.FromArgb(
            control.Color.A,
            control.Color.R,
            control.Color.G,
            (byte)e.NewValue);

        control.SetColorFromComponent(color);
    }

    private static void PropertyAlphaValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        var color = Color.FromArgb(
            (byte)e.NewValue,
            control.Color.R,
            control.Color.G,
            control.Color.B);

        control.SetColorFromComponent(color);
    }

    private static void PropertyHueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        control.SetColorFromHsv();
    }

    private static void PropertySaturationValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        control.SetColorFromHsv();
    }

    private static void PropertyValueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPickerControl)d;

        if (control._isSynchronizing) return;

        control.SetColorFromHsv();
    }

    private void ColorPickerControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        SynchronizeFromColor(Color);
    }

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        LabelRedChannel = ColorManagementResources.LabelRedChannel;
        LabelGreenChannel = ColorManagementResources.LabelGreenChannel;
        LabelBlueChannel = ColorManagementResources.LabelBlueChannel;
        LabelHueChannel = ColorManagementResources.LabelHueChannel;
        LabelSaturationChannel = ColorManagementResources.LabelSaturationChannel;
        LabelValueChannel = ColorManagementResources.LabelValueChannel;
        LabelAlphaChannel = ColorManagementResources.LabelAlphaChannel;
        LabelHexadecimalCode = ColorManagementResources.LabelHexadecimalCode;
        LabelPreview = ColorManagementResources.LabelPreview;
    }

    private void SetColorFromComponent(Color color)
    {
        Color = color;
        SynchronizeFromColor(color);
    }

    private void SetColorFromHsv()
    {
        var alpha = AlphaValue;
        var color = ColorExtensions.ToColor(HueValue, SaturationValue, ValueValue);
        color = Color.FromArgb(alpha, color.R, color.G, color.B);

        Color = color;
        SynchronizeFromColor(color);
    }

    private void SynchronizeFromColor(Color color)
    {
        _isSynchronizing = true;

        try
        {
            RedValue = color.R;
            GreenValue = color.G;
            BlueValue = color.B;
            AlphaValue = color.A;

            var hsv = color.ToHsv();

            HueValue = hsv.Hue;
            SaturationValue = hsv.Saturation;
            ValueValue = hsv.Value;

            UpdateGradientSlider(color);
        }
        finally
        {
            _isSynchronizing = false;
        }
    }

    private void UpdateGradientSlider(Color color)
    {
        if (!IsLoaded) return;

        RedGradientStart.Color = Color.FromArgb(255, 0, color.G, color.B);
        RedGradientStop.Color = Color.FromArgb(255, 255, color.G, color.B);

        GreenGradientStart.Color = Color.FromArgb(255, color.R, 0, color.B);
        GreenGradientStop.Color = Color.FromArgb(255, color.R, 255, color.B);

        BlueGradientStart.Color = Color.FromArgb(255, color.R, color.G, 0);
        BlueGradientStop.Color = Color.FromArgb(255, color.R, color.G, 255);

        AlphaGradientStart.Color = Color.FromArgb(0, color.R, color.G, color.B);
        AlphaGradientStop.Color = Color.FromArgb(255, color.R, color.G, color.B);

        var hsv = color.ToHsv();

        SaturationGradientStart.Color = ColorExtensions.ToColor(hsv.Hue, 0, hsv.Value);
        SaturationGradientStop.Color = ColorExtensions.ToColor(hsv.Hue, 1, hsv.Value);

        ValueGradientStart.Color = ColorExtensions.ToColor(hsv.Hue, hsv.Saturation, 0);
        ValueGradientStop.Color = ColorExtensions.ToColor(hsv.Hue, hsv.Saturation, 1);
    }

    private void UIElement_int_only_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsDigit);
    }

    private void TextBoxBase_0_to_255_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (string.IsNullOrWhiteSpace(textBox.Text)) return;
        if (!int.TryParse(textBox.Text, out var value)) return;

        var correctedValue = value switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => value
        };

        if (correctedValue == value) return;

        textBox.Text = correctedValue.ToString(CultureInfo.InvariantCulture);
        textBox.CaretIndex = textBox.Text.Length;
    }

    private void UIElement_double_only_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text.Insert(textBox.SelectionStart, e.Text);

        e.Handled = !text.IsOnlyDecimal();
    }

    private void TextBoxBase_0_to_360_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ClampDoubleTextBoxValue(sender, 0d, 360d);
    }

    private void TextBoxBase_0_to_1_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ClampDoubleTextBoxValue(sender, 0d, 1d);
    }

    private static void ClampDoubleTextBoxValue(object sender, double min, double max)
    {
        if (sender is not TextBox textBox) return;
        if (string.IsNullOrWhiteSpace(textBox.Text)) return;

        var decimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;

        if (textBox.Text.EndsWith(decimalSeparator, StringComparison.Ordinal))
        {
            return;
        }

        if (!double.TryParse(textBox.Text, CultureInfo.InvariantCulture, out var value)) return;

        var correctedValue = value switch
        {
            var v when v > max => max,
            var v when v < min => min,
            _ => value
        };

        if (Math.Abs(correctedValue - value) < 0.0001) return;

        textBox.Text = correctedValue.ToString(CultureInfo.InvariantCulture);
        textBox.CaretIndex = textBox.Text.Length;
    }
}