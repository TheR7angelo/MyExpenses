using System.Windows;
using System.Windows.Media;

namespace MyExpenses.Wpf.UserControls;

public partial class ColorPickerControl
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color),
        typeof(ColorPickerControl), new PropertyMetadata(default(Color), PropertyColor_OnChangedCallback));

    private static void PropertyColor_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        sender.UpdateGradiantSlider();

        var newColor = (Color)e.NewValue;
        sender.RedValue = newColor.R;
        sender.GreenValue = newColor.G;
        sender.BlueValue = newColor.B;
        sender.AlphaValue = newColor.A;
    }

    public static readonly DependencyProperty RedSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(RedSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty RedSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(RedSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty RedValueProperty = DependencyProperty.Register(nameof(RedValue),
        typeof(byte), typeof(ColorPickerControl), new PropertyMetadata(default(byte), PropertyRedValue_OnChangedCallback));

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
        typeof(byte), typeof(ColorPickerControl), new PropertyMetadata(default(byte), PropertyGreenValue_OnChangedCallback));

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
        typeof(byte), typeof(ColorPickerControl), new PropertyMetadata(default(byte), PropertyBlueValue_OnChangedCallback));

    private static void PropertyBlueValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb(sender.Color.A, sender.Color.R, sender.Color.G, (byte)e.NewValue);
        sender.Color = color;
    }

    public static readonly DependencyProperty AlphaSliderBorderThicknessProperty =
        DependencyProperty.Register(nameof(AlphaSliderBorderThickness), typeof(Thickness), typeof(ColorPickerControl),
            new PropertyMetadata(default(Thickness)));

    public static readonly DependencyProperty AlphaSliderBorderBrushProperty =
        DependencyProperty.Register(nameof(AlphaSliderBorderBrush), typeof(Brush), typeof(ColorPickerControl),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty AlphaValueProperty = DependencyProperty.Register(nameof(AlphaValue),
        typeof(byte), typeof(ColorPickerControl), new PropertyMetadata(default(byte), PropertyAlphaValue_OnChangedCallback));

    private static void PropertyAlphaValue_OnChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = (ColorPickerControl)d;
        var color = Color.FromArgb((byte)e.NewValue, sender.Color.R, sender.Color.G, sender.Color.G);
        sender.Color = color;
    }

    public ColorPickerControl()
    {
        InitializeComponent();
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
    }
}