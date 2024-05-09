using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyExpenses.Wpf.Resources.Regex;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.UserControls;

public partial class ColorPickerControl
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color),
        typeof(ColorPickerControl), new PropertyMetadata(Colors.White, PropertyColor_OnChangedCallback));

    private static void PropertyColor_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;

        var newColor = (Color)e.NewValue;
        sender.InitializeValue(newColor);
    }

    public static readonly DependencyProperty RedSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(RedSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty RedSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(RedSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty RedValueProperty = DependencyProperty.Register(nameof(RedValue),
        typeof(byte), typeof(ColorPickerControl),
        new PropertyMetadata(default(byte), PropertyRedValue_OnChangedCallback));

    private static void PropertyRedValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb(sender.Color.A, (byte)e.NewValue, sender.Color.G, sender.Color.B);
        sender.Color = color;
    }

    public static readonly DependencyProperty GreenSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(GreenSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty GreenSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(GreenSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty GreenValueProperty = DependencyProperty.Register(nameof(GreenValue),
        typeof(byte), typeof(ColorPickerControl),
        new PropertyMetadata(default(byte), PropertyGreenValue_OnChangedCallback));

    private static void PropertyGreenValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb(sender.Color.A, sender.Color.R, (byte)e.NewValue, sender.Color.B);
        sender.Color = color;
    }

    public static readonly DependencyProperty BlueSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(BlueSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty BlueSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(BlueSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BlueValueProperty = DependencyProperty.Register(nameof(BlueValue),
        typeof(byte), typeof(ColorPickerControl),
        new PropertyMetadata(default(byte), PropertyBlueValue_OnChangedCallback));

    private static void PropertyBlueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb(sender.Color.A, sender.Color.R, sender.Color.G, (byte)e.NewValue);
        sender.Color = color;
    }

    public static readonly DependencyProperty HueSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(HueSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty HueSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(HueSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty HueValueProperty = DependencyProperty.Register(nameof(HueValue),
        typeof(double), typeof(ColorPickerControl),
        new PropertyMetadata(default(double), PropertyHueValue_OnChangedCallback));

    private static void PropertyHueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        sender.UpdateHsvValue();
    }

    public static readonly DependencyProperty SaturationSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(SaturationSliderBorderThickness), typeof(Thickness),
            typeof(ColorPickerControl), new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty SaturationSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(SaturationSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty SaturationValueProperty =
        DependencyProperty.Register(nameof(SaturationValue), typeof(double), typeof(ColorPickerControl),
            new PropertyMetadata(default(double), PropertySaturation_OnChangedCallback));

    private static void PropertySaturation_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        sender.UpdateHsvValue();
    }

    public static readonly DependencyProperty ValueSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(ValueSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty ValueSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(ValueSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ValueValueProperty = DependencyProperty.Register(nameof(ValueValue),
        typeof(double), typeof(ColorPickerControl),
        new PropertyMetadata(default(double), PropertyValueValue_OnChangedCallback));

    private static void PropertyValueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        sender.UpdateHsvValue();
    }

    public static readonly DependencyProperty AlphaSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(AlphaSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty AlphaSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(AlphaSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty AlphaValueProperty = DependencyProperty.Register(nameof(AlphaValue),
        typeof(byte), typeof(ColorPickerControl),
        new PropertyMetadata(default(byte), PropertyAlphaValue_OnChangedCallback));

    public static readonly DependencyProperty LabelRedChannelProperty =
        DependencyProperty.Register(nameof(LabelRedChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("R"));

    public static readonly DependencyProperty LabelGreenChannelProperty =
        DependencyProperty.Register(nameof(LabelGreenChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("G"));

    public static readonly DependencyProperty LabelBlueChannelProperty =
        DependencyProperty.Register(nameof(LabelBlueChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("B"));

    public static readonly DependencyProperty LabelHueChannelProperty =
        DependencyProperty.Register(nameof(LabelHueChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("H"));

    public static readonly DependencyProperty LabelSaturationChannelProperty =
        DependencyProperty.Register(nameof(LabelSaturationChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("S"));

    public static readonly DependencyProperty LabelValueChannelProperty =
        DependencyProperty.Register(nameof(LabelValueChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("V"));

    public static readonly DependencyProperty LabelAlphaChannelProperty =
        DependencyProperty.Register(nameof(LabelAlphaChannel), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("A"));

    public static readonly DependencyProperty LabelPreviewProperty = DependencyProperty.Register(nameof(LabelPreview),
        typeof(string), typeof(ColorPickerControl), new PropertyMetadata("Preview :"));

    public static readonly DependencyProperty RectanglePreviewThicknessProperty =
        DependencyProperty.Register(nameof(RectanglePreviewThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty RectanglePreviewStrokeProperty =
        DependencyProperty.Register(nameof(RectanglePreviewStroke), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty LabelHexadecimalCodeProperty =
        DependencyProperty.Register(nameof(LabelHexadecimalCode), typeof(string), typeof(ColorPickerControl),
            new PropertyMetadata("Hexadecimal code :"));

    private static void PropertyAlphaValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb((byte)e.NewValue, sender.Color.R, sender.Color.G, sender.Color.G);
        sender.Color = color;
    }

    public event EventHandler<ColorChangedEventArgs>? ColorChanged;

    public void ChangeColor()
        => OnColorChanged();

    protected virtual void OnColorChanged()
    {
        ColorChanged?.Invoke(this, new ColorChangedEventArgs(Color));
    }

    public ColorPickerControl()
    {
        InitializeComponent();
        InitializeValue();
    }

    private void UpdateHsvValue()
    {
        var hue = HueValue;
        var saturation = SaturationValue;
        var value = ValueValue;

        var color = ColorExtensions.ToColor(hue, saturation, value);
        Color = color;
    }

    private void InitializeValue(Color? color = null)
    {
        var isNewColor = color is null;
        var newColor = color ?? Color;

        RedValue = newColor.R;
        GreenValue = newColor.G;
        BlueValue = newColor.B;
        AlphaValue = newColor.A;

        var (hue, saturation, value) = newColor.ToHsv();
        HueValue = hue;
        SaturationValue = saturation;
        ValueValue = value;

        UpdateGradiantSlider();
        if (isNewColor) ChangeColor();
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public Thickness RedSliderBorderThickness
    {
        get => (Thickness)GetValue(RedSliderBorderThicknessProperty);
        set => SetValue(RedSliderBorderThicknessProperty, value);
    }

    public Brush RedSliderBorderBrush
    {
        get => (Brush)GetValue(RedSliderBorderBrushProperty);
        set => SetValue(RedSliderBorderBrushProperty, value);
    }

    public byte RedValue
    {
        get => (byte)GetValue(RedValueProperty);
        set => SetValue(RedValueProperty, value);
    }

    public Thickness GreenSliderBorderThickness
    {
        get => (Thickness)GetValue(GreenSliderBorderThicknessProperty);
        set => SetValue(GreenSliderBorderThicknessProperty, value);
    }

    public Brush GreenSliderBorderBrush
    {
        get => (Brush)GetValue(GreenSliderBorderBrushProperty);
        set => SetValue(GreenSliderBorderBrushProperty, value);
    }

    public byte GreenValue
    {
        get => (byte)GetValue(GreenValueProperty);
        set => SetValue(GreenValueProperty, value);
    }

    public Thickness BlueSliderBorderThickness
    {
        get => (Thickness)GetValue(BlueSliderBorderThicknessProperty);
        set => SetValue(BlueSliderBorderThicknessProperty, value);
    }

    public Brush BlueSliderBorderBrush
    {
        get => (Brush)GetValue(BlueSliderBorderBrushProperty);
        set => SetValue(BlueSliderBorderBrushProperty, value);
    }

    public byte BlueValue
    {
        get => (byte)GetValue(BlueValueProperty);
        set => SetValue(BlueValueProperty, value);
    }

    public Thickness AlphaSliderBorderThickness
    {
        get => (Thickness)GetValue(AlphaSliderBorderThicknessProperty);
        set => SetValue(AlphaSliderBorderThicknessProperty, value);
    }

    public Brush AlphaSliderBorderBrush
    {
        get => (Brush)GetValue(AlphaSliderBorderBrushProperty);
        set => SetValue(AlphaSliderBorderBrushProperty, value);
    }

    public byte AlphaValue
    {
        get => (byte)GetValue(AlphaValueProperty);
        set => SetValue(AlphaValueProperty, value);
    }

    public Brush HueSliderBorderBrush
    {
        get => (Brush)GetValue(HueSliderBorderBrushProperty);
        set => SetValue(HueSliderBorderBrushProperty, value);
    }

    public Thickness HueSliderBorderThickness
    {
        get => (Thickness)GetValue(HueSliderBorderThicknessProperty);
        set => SetValue(HueSliderBorderThicknessProperty, value);
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

    public Brush SaturationSliderBorderBrush
    {
        get => (Brush)GetValue(SaturationSliderBorderBrushProperty);
        set => SetValue(SaturationSliderBorderBrushProperty, value);
    }

    public Thickness SaturationSliderBorderThickness
    {
        get => (Thickness)GetValue(SaturationSliderBorderThicknessProperty);
        set => SetValue(SaturationSliderBorderThicknessProperty, value);
    }

    public Brush ValueSliderBorderBrush
    {
        get => (Brush)GetValue(ValueSliderBorderBrushProperty);
        set => SetValue(ValueSliderBorderBrushProperty, value);
    }

    public Thickness ValueSliderBorderThickness
    {
        get => (Thickness)GetValue(ValueSliderBorderThicknessProperty);
        set => SetValue(ValueSliderBorderThicknessProperty, value);
    }

    public double ValueValue
    {
        get => (double)GetValue(ValueValueProperty);
        set => SetValue(ValueValueProperty, value);
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

    public Thickness RectanglePreviewThickness
    {
        get => (Thickness)GetValue(RectanglePreviewThicknessProperty);
        set => SetValue(RectanglePreviewThicknessProperty, value);
    }

    public Brush RectanglePreviewStroke
    {
        get => (Brush)GetValue(RectanglePreviewStrokeProperty);
        set => SetValue(RectanglePreviewStrokeProperty, value);
    }

    public string LabelHexadecimalCode
    {
        get => (string)GetValue(LabelHexadecimalCodeProperty);
        set => SetValue(LabelHexadecimalCodeProperty, value);
    }

    private void UpdateGradiantSlider()
    {
        var redGradientStart = Color.FromArgb(255, 0, Color.G, Color.B);
        var redGradientStop = Color.FromArgb(255, 255, Color.G, Color.B);
        RedGradientStart.Color = redGradientStart;
        RedGradientStop.Color = redGradientStop;

        var greenGradientStart = Color.FromArgb(255, Color.R, 0, Color.B);
        var greenGradientStop = Color.FromArgb(255, Color.R, 255, Color.B);
        GreenGradientStart.Color = greenGradientStart;
        GreenGradientStop.Color = greenGradientStop;

        var blueGradientStart = Color.FromArgb(255, Color.R, Color.G, 0);
        var blueGradientStop = Color.FromArgb(255, Color.R, Color.G, 255);
        BlueGradientStart.Color = blueGradientStart;
        BlueGradientStop.Color = blueGradientStop;

        var alphaGradientStart = Color.FromArgb(0, Color.R, Color.G, Color.B);
        var alphaGradientStop = Color.FromArgb(255, Color.R, Color.G, Color.B);
        AlphaGradientStart.Color = alphaGradientStart;
        AlphaGradientStop.Color = alphaGradientStop;

        var (hue, saturation, value) = Color.ToHsv();
        SaturationGradientStart.Color = ColorExtensions.ToColor(hue, 0, value);
        SaturationGradientStop.Color = ColorExtensions.ToColor(hue, 1, value);

        ValueGradientStart.Color = ColorExtensions.ToColor(hue, saturation, 0);
        ValueGradientStop.Color = ColorExtensions.ToColor(hue, saturation, 1);
    }

    private void UIElement_int_only_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !e.Text.All(char.IsDigit);
    }

    private void TextBoxBase_0_to_255_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text)) return;
        var nbr = int.Parse(textBox.Text);
        var oldNbr = nbr;
        nbr = nbr switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => nbr
        };

        textBox.Text = nbr.ToString();
        if (nbr != oldNbr) textBox.CaretIndex = textBox.Text.Length;
    }

    private void UIElement_double_only_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = e.Text.IsOnlyDecimal();
    }

    private void TextBoxBase_0_to_360_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text)) return;

        var lastCharacter = textBox.Text[^1].ToString();
        if (lastCharacter == CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
        {
            textBox.Text = textBox.Text[..^1];
            textBox.CaretIndex = textBox.Text.Length;
        }

        var nbr = double.Parse(textBox.Text, CultureInfo.InvariantCulture);
        var oldNbr = nbr;
        nbr = nbr switch
        {
            > 360 => 360,
            < 0 => 0,
            _ => nbr
        };

        textBox.Text = nbr.ToString(CultureInfo.InvariantCulture);
        if (Math.Abs(nbr - oldNbr) > 0.0001) textBox.CaretIndex = textBox.Text.Length;
    }

    private void TextBoxBase_0_to_1_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (string.IsNullOrEmpty(textBox.Text)) return;

        var lastCharacter = textBox.Text[^1].ToString();
        if (lastCharacter == CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator)
        {
            textBox.Text = textBox.Text[..^1];
            textBox.CaretIndex = textBox.Text.Length;
        }

        var nbr = double.Parse(textBox.Text, CultureInfo.InvariantCulture);
        var oldNbr = nbr;
        nbr = nbr switch
        {
            > 1 => 1,
            < 0 => 0,
            _ => nbr
        };

        textBox.Text = nbr.ToString(CultureInfo.InvariantCulture);
        if (Math.Abs(nbr - oldNbr) > 0.0001) textBox.CaretIndex = textBox.Text.Length;
    }
}