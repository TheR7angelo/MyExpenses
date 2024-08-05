using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.UserControls;
using MyExpenses.Wpf.Resources.Resx.UserControls.Settings.AppearanceControl;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using Theme = MaterialDesignThemes.Wpf.Theme;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class AppearanceControl
{
    #region Properties

    #region DependencyProperties

    public static readonly DependencyProperty LabelThemeModeProperty =
        DependencyProperty.Register(nameof(LabelThemeMode), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelThemeMode
    {
        get => (string)GetValue(LabelThemeModeProperty);
        set => SetValue(LabelThemeModeProperty, value);
    }

    public static readonly DependencyProperty CheckBoxContentSyncWithOsProperty =
        DependencyProperty.Register(nameof(CheckBoxContentSyncWithOs), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string CheckBoxContentSyncWithOs
    {
        get => (string)GetValue(CheckBoxContentSyncWithOsProperty);
        set => SetValue(CheckBoxContentSyncWithOsProperty, value);
    }

    public static readonly DependencyProperty LabelContentPrimaryLightProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryLight), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryLight
    {
        get => (string)GetValue(LabelContentPrimaryLightProperty);
        set => SetValue(LabelContentPrimaryLightProperty, value);
    }

    public static readonly DependencyProperty LabelContentPrimaryMindProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryMind), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryMind
    {
        get => (string)GetValue(LabelContentPrimaryMindProperty);
        set => SetValue(LabelContentPrimaryMindProperty, value);
    }

    public static readonly DependencyProperty LabelContentPrimaryDarkProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryDark), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryDark
    {
        get => (string)GetValue(LabelContentPrimaryDarkProperty);
        set => SetValue(LabelContentPrimaryDarkProperty, value);
    }

    public static readonly DependencyProperty LabelContentSecondaryLightProperty =
        DependencyProperty.Register(nameof(LabelContentSecondaryLight), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentSecondaryLight
    {
        get => (string)GetValue(LabelContentSecondaryLightProperty);
        set => SetValue(LabelContentSecondaryLightProperty, value);
    }

    public static readonly DependencyProperty LabelContentSecondaryMindProperty =
        DependencyProperty.Register(nameof(LabelContentSecondaryMind), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentSecondaryMind
    {
        get => (string)GetValue(LabelContentSecondaryMindProperty);
        set => SetValue(LabelContentSecondaryMindProperty, value);
    }

    public string LabelContentSecondaryDark
    {
        get => (string)GetValue(LabelContentSecondaryDarkProperty);
        set => SetValue(LabelContentSecondaryDarkProperty, value);
    }

    public static readonly DependencyProperty LabelContentSecondaryDarkProperty =
        DependencyProperty.Register(nameof(LabelContentSecondaryDark), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ButtonContentPrimaryColorProperty =
        DependencyProperty.Register(nameof(ButtonContentPrimaryColor), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string ButtonContentPrimaryColor
    {
        get => (string)GetValue(ButtonContentPrimaryColorProperty);
        set => SetValue(ButtonContentPrimaryColorProperty, value);
    }

    public static readonly DependencyProperty ButtonContentSecondaryColorProperty =
        DependencyProperty.Register(nameof(ButtonContentSecondaryColor), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string ButtonContentSecondaryColor
    {
        get => (string)GetValue(ButtonContentSecondaryColorProperty);
        set => SetValue(ButtonContentSecondaryColorProperty, value);
    }

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

        UpdateLanguage();
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        InitializeComponent();
    }

    #region Action

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

    private void CheckBoxSyncWithOs_OnChecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void CheckBoxSyncWithOs_OnUnchecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void ToggleButtonLightDark_OnChecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void ToggleButtonLightDark_OnUnchecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    #endregion

    #region Function

    private static Color GetNewColor(Color defaultColor, string title)
    {
        var colorPickerWindow = new ColorPickerWindow { Title = title };
        colorPickerWindow.ColorPickerControl.InitializeValue(defaultColor);
        colorPickerWindow.ShowDialog();

        if (colorPickerWindow.DialogResult is not true) return defaultColor;

        var newColor = colorPickerWindow.ColorResult ?? defaultColor;
        return newColor;
    }

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

    private void UpdateLanguage()
    {
        LabelThemeMode = AppearanceControlResources.LabelThemeMode;
        CheckBoxContentSyncWithOs = AppearanceControlResources.CheckBoxContentSyncWithOs;

        LabelContentPrimaryLight = AppearanceControlResources.LabelContentPrimaryLight;
        LabelContentPrimaryMind = AppearanceControlResources.LabelContentPrimaryMind;
        LabelContentPrimaryDark = AppearanceControlResources.LabelContentPrimaryDark;
        LabelContentSecondaryLight = AppearanceControlResources.LabelContentSecondaryLight;
        LabelContentSecondaryMind = AppearanceControlResources.LabelContentSecondaryMind;
        LabelContentSecondaryDark = AppearanceControlResources.LabelContentSecondaryDark;

        ButtonContentPrimaryColor = AppearanceControlResources.ButtonContentPrimaryColor;
        ButtonContentSecondaryColor = AppearanceControlResources.ButtonContentSecondaryColor;
    }

    private void UpdatePrimaryLabelTheme()
    {
        BackgroundPrimaryLightBrush = new SolidColorBrush(Theme.PrimaryLight.Color);
        BackgroundPrimaryMindBrush = new SolidColorBrush(Theme.PrimaryMid.Color);
        BackgroundPrimaryDarkBrush = new SolidColorBrush(Theme.PrimaryDark.Color);
    }

    private void UpdateSecondaryLabelTheme()
    {
        BackgroundSecondaryLightBrush = new SolidColorBrush(Theme.SecondaryLight.Color);
        BackgroundSecondaryMindBrush = new SolidColorBrush(Theme.SecondaryMid.Color);
        BackgroundSecondaryDarkBrush = new SolidColorBrush(Theme.SecondaryDark.Color);
    }

    #endregion
}