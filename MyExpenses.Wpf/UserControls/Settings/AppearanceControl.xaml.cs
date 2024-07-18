using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.UserControls;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using Theme = MaterialDesignThemes.Wpf.Theme;

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

    public static readonly DependencyProperty SyncWithOsProperty = DependencyProperty.Register(nameof(SyncWithOs),
        typeof(bool), typeof(AppearanceControl), new PropertyMetadata(default(bool)));

    public bool SyncWithOs
    {
        get => (bool)GetValue(SyncWithOsProperty);
        set => SetValue(SyncWithOsProperty, value);
    }

    public static readonly DependencyProperty LightDarkProperty = DependencyProperty.Register(nameof(LightDark),
        typeof(bool), typeof(AppearanceControl), new PropertyMetadata(default(bool)));

    public bool LightDark
    {
        get => (bool)GetValue(LightDarkProperty);
        set => SetValue(LightDarkProperty, value);
    }

    public Brush BackgroundSecondaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryDarkBrushProperty);
        set => SetValue(BackgroundSecondaryDarkBrushProperty, value);
    }

    #endregion

    public Theme Theme { get; }

    #endregion

    public AppearanceControl()
    {
        var configuration = MyExpenses.Utils.Config.Configuration;
        var baseTheme = (BaseTheme)configuration.Interface.Theme.BaseTheme;
        var primaryColor = (Color)configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToColor()!;
        var secondaryColor = (Color)configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToColor()!;

        Theme = new Theme();
        Theme.SetBaseTheme(baseTheme);
        Theme.SetPrimaryColor(primaryColor);
        Theme.SetSecondaryColor(secondaryColor);

        UpdatePrimaryLabelTheme();
        UpdateSecondaryLabelTheme();

        if (baseTheme is BaseTheme.Inherit) SyncWithOs = true;
        else if (baseTheme is BaseTheme.Dark) LightDark = true;

        InitializeComponent();
    }

    private void UpdateSecondaryLabelTheme()
    {
        BackgroundSecondaryLightBrush = new SolidColorBrush(Theme.SecondaryLight.Color);
        BackgroundSecondaryMindBrush = new SolidColorBrush(Theme.SecondaryMid.Color);
        BackgroundSecondaryDarkBrush = new SolidColorBrush(Theme.SecondaryDark.Color);
    }

    private void UpdatePrimaryLabelTheme()
    {
        BackgroundPrimaryLightBrush = new SolidColorBrush(Theme.PrimaryLight.Color);
        BackgroundPrimaryMindBrush = new SolidColorBrush(Theme.PrimaryMid.Color);
        BackgroundPrimaryDarkBrush = new SolidColorBrush(Theme.PrimaryDark.Color);
    }

    private void ButtonPrimaryColor_OnClick(object sender, RoutedEventArgs e)
    {
        var title = AppearanceControlResources.ButtonPrimaryColorTitle;

        var color = Theme.PrimaryMid.Color;
        var newColor = GetNewColor(color, title);

        Theme.SetPrimaryColor(newColor);
        UpdatePrimaryLabelTheme();
    }

    private void ButtonSecondaryColor_OnClick(object sender, RoutedEventArgs e)
    {
        var title = AppearanceControlResources.ButtonSecondaryColorTitle;

        var color = Theme.SecondaryMid.Color;
        var newColor = GetNewColor(color, title);

        Theme.SetSecondaryColor(newColor);
        UpdateSecondaryLabelTheme();
    }

    private static Color GetNewColor(Color defaultColor, string title)
    {
        var colorPickerWindow = new ColorPickerWindow { Title = title };
        colorPickerWindow.ColorPickerControl.InitializeValue(defaultColor);
        colorPickerWindow.ShowDialog();

        if (colorPickerWindow.DialogResult is not true) return defaultColor;

        var newColor = colorPickerWindow.ColorResult ?? defaultColor;
        return newColor;
    }

    private void CheckBoxSyncWithOs_OnChecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void CheckBoxSyncWithOs_OnUnchecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void ToggleButtonLightDark_OnChecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void ToggleButtonLightDark_OnUnchecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void UpdateBaseTheme()
    {
        var configuration = MyExpenses.Utils.Config.Configuration;
        var paletteHelper = new PaletteHelper();

        configuration.Interface.Theme.BaseTheme = SyncWithOs ? (EBaseTheme)BaseTheme.Inherit :
            LightDark is false ? (EBaseTheme)BaseTheme.Light : (EBaseTheme)BaseTheme.Dark;

        var theme = paletteHelper.GetTheme();
        theme.SetBaseTheme((BaseTheme)configuration.Interface.Theme.BaseTheme);

        paletteHelper.SetTheme(theme);
    }
}