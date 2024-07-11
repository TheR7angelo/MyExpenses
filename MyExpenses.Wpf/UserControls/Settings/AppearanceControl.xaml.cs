using System.Windows;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class AppearanceControl
{
    #region Properties

    #region DependencyProperties

    public static readonly DependencyProperty BackgroundPrimaryMindBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryMindBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryMindBrushProperty);
        set => SetValue(BackgroundPrimaryMindBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundPrimaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryLightBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryLightBrushProperty);
        set => SetValue(BackgroundPrimaryLightBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundPrimaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryDarkBrushProperty);
        set => SetValue(BackgroundPrimaryDarkBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundSecondaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundSecondaryLightBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryLightBrushProperty);
        set => SetValue(BackgroundSecondaryLightBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundSecondaryMindBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundSecondaryMindBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryMindBrushProperty);
        set => SetValue(BackgroundSecondaryMindBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundSecondaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundSecondaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryDarkBrushProperty);
        set => SetValue(BackgroundSecondaryDarkBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundPrimaryMindBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundPrimaryMindBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryMindBrushProperty);
        set => SetValue(ForegroundPrimaryMindBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundPrimaryLightBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundPrimaryLightBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryLightBrushProperty);
        set => SetValue(ForegroundPrimaryLightBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundPrimaryDarkBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundPrimaryDarkBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryDarkBrushProperty);
        set => SetValue(ForegroundPrimaryDarkBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundSecondaryLightBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundSecondaryLightBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryLightBrushProperty);
        set => SetValue(ForegroundSecondaryLightBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundSecondaryMindBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundSecondaryMindBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryMindBrushProperty);
        set => SetValue(ForegroundSecondaryMindBrushProperty, value);
    }

    public static readonly DependencyProperty ForegroundSecondaryDarkBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush ForegroundSecondaryDarkBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryDarkBrushProperty);
        set => SetValue(ForegroundSecondaryDarkBrushProperty, value);
    }

    #endregion

    public Theme Theme { get; }

    #endregion

    public AppearanceControl()
    {
        var configuration = MyExpenses.Utils.Config.Configuration;
        var primaryColor = configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToColor();
        var secondaryColor = configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToColor();

        Theme = new Theme();
        if (primaryColor is not null && secondaryColor is not null)
        {
            Theme.SetPrimaryColor((Color)primaryColor);
            Theme.SetSecondaryColor((Color)secondaryColor);
        }

        UpdatePrimaryLabelTheme();
        UpdateSecondaryLabelTheme();

        InitializeComponent();
    }

    private void UpdateSecondaryLabelTheme()
    {
        BackgroundSecondaryLightBrush = new SolidColorBrush(Theme.SecondaryLight.Color);
        BackgroundSecondaryMindBrush = new SolidColorBrush(Theme.SecondaryMid.Color);
        BackgroundSecondaryDarkBrush = new SolidColorBrush(Theme.SecondaryDark.Color);

        ForegroundSecondaryLightBrush = new SolidColorBrush(Theme.SecondaryLight.Color.ContrastingForegroundColor());
        ForegroundSecondaryMindBrush = new SolidColorBrush(Theme.SecondaryMid.Color.ContrastingForegroundColor());
        ForegroundSecondaryDarkBrush = new SolidColorBrush(Theme.SecondaryDark.Color.ContrastingForegroundColor());
    }

    private void UpdatePrimaryLabelTheme()
    {
        BackgroundPrimaryLightBrush = new SolidColorBrush(Theme.PrimaryLight.Color);
        BackgroundPrimaryMindBrush = new SolidColorBrush(Theme.PrimaryMid.Color);
        BackgroundPrimaryDarkBrush = new SolidColorBrush(Theme.PrimaryDark.Color);

        ForegroundPrimaryLightBrush = new SolidColorBrush(Theme.PrimaryLight.Color.ContrastingForegroundColor());
        ForegroundPrimaryMindBrush = new SolidColorBrush(Theme.PrimaryMid.Color.ContrastingForegroundColor());
        ForegroundPrimaryDarkBrush = new SolidColorBrush(Theme.PrimaryDark.Color.ContrastingForegroundColor());
    }

    private void ButtonPrimaryColor_OnClick(object sender, RoutedEventArgs e)
    {
        var color = Theme.PrimaryMid.Color;
        var newColor = GetNewColor(color);

        Theme.SetPrimaryColor(newColor);
        UpdatePrimaryLabelTheme();
    }

    private void ButtonSecondaryColor_OnClick(object sender, RoutedEventArgs e)
    {
        var color = Theme.SecondaryMid.Color;
        var newColor = GetNewColor(color);

        Theme.SetSecondaryColor(newColor);
        UpdateSecondaryLabelTheme();
    }

    private static Color GetNewColor(Color defaultColor)
    {
        var colorPickerWindow = new ColorPickerWindow();
        colorPickerWindow.ColorPickerControl.InitializeValue(defaultColor);
        colorPickerWindow.ShowDialog();

        if (colorPickerWindow.DialogResult is not true) return defaultColor;

        var newColor = colorPickerWindow.ColorResult ?? defaultColor;
        return newColor;
    }
}