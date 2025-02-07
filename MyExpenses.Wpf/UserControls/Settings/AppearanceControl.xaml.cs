using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.UserControls.Settings.AppearanceControl;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows;
using Theme = MaterialDesignThemes.Wpf.Theme;

namespace MyExpenses.Wpf.UserControls.Settings;

public partial class AppearanceControl
{
    #region Properties

    #region DependencyProperties

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelThemeModeProperty =
        DependencyProperty.Register(nameof(LabelThemeMode), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelThemeMode
    {
        get => (string)GetValue(LabelThemeModeProperty);
        set => SetValue(LabelThemeModeProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CheckBoxContentSyncWithOsProperty =
        DependencyProperty.Register(nameof(CheckBoxContentSyncWithOs), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string CheckBoxContentSyncWithOs
    {
        get => (string)GetValue(CheckBoxContentSyncWithOsProperty);
        set => SetValue(CheckBoxContentSyncWithOsProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelContentPrimaryLightProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryLight), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryLight
    {
        get => (string)GetValue(LabelContentPrimaryLightProperty);
        set => SetValue(LabelContentPrimaryLightProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelContentPrimaryMindProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryMind), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryMind
    {
        get => (string)GetValue(LabelContentPrimaryMindProperty);
        set => SetValue(LabelContentPrimaryMindProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelContentPrimaryDarkProperty =
        DependencyProperty.Register(nameof(LabelContentPrimaryDark), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentPrimaryDark
    {
        get => (string)GetValue(LabelContentPrimaryDarkProperty);
        set => SetValue(LabelContentPrimaryDarkProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelContentSecondaryLightProperty =
        DependencyProperty.Register(nameof(LabelContentSecondaryLight), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string LabelContentSecondaryLight
    {
        get => (string)GetValue(LabelContentSecondaryLightProperty);
        set => SetValue(LabelContentSecondaryLightProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelContentSecondaryDarkProperty =
        DependencyProperty.Register(nameof(LabelContentSecondaryDark), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentPrimaryColorProperty =
        DependencyProperty.Register(nameof(ButtonContentPrimaryColor), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string ButtonContentPrimaryColor
    {
        get => (string)GetValue(ButtonContentPrimaryColorProperty);
        set => SetValue(ButtonContentPrimaryColorProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentSecondaryColorProperty =
        DependencyProperty.Register(nameof(ButtonContentSecondaryColor), typeof(string), typeof(AppearanceControl),
            new PropertyMetadata(default(string)));

    public string ButtonContentSecondaryColor
    {
        get => (string)GetValue(ButtonContentSecondaryColorProperty);
        set => SetValue(ButtonContentSecondaryColorProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundPrimaryMindBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryMindBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryMindBrushProperty);
        set => SetValue(BackgroundPrimaryMindBrushProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundPrimaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryLightBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryLightBrushProperty);
        set => SetValue(BackgroundPrimaryLightBrushProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundPrimaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundPrimaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryDarkBrushProperty);
        set => SetValue(BackgroundPrimaryDarkBrushProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundSecondaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryLightBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundSecondaryLightBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryLightBrushProperty);
        set => SetValue(BackgroundSecondaryLightBrushProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundSecondaryMindBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryMindBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    public Brush BackgroundSecondaryMindBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryMindBrushProperty);
        set => SetValue(BackgroundSecondaryMindBrushProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty BackgroundSecondaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryDarkBrush), typeof(Brush), typeof(AppearanceControl),
            new PropertyMetadata(default(Brush)));

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SyncWithOsProperty = DependencyProperty.Register(nameof(SyncWithOs),
        typeof(bool), typeof(AppearanceControl), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool SyncWithOs
    {
        get => (bool)GetValue(SyncWithOsProperty);
        set => SetValue(SyncWithOsProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LightDarkProperty = DependencyProperty.Register(nameof(LightDark),
        typeof(bool), typeof(AppearanceControl), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        Theme = new Theme();
        Theme.SetBaseTheme(baseTheme);
        Theme.SetPrimaryColor(primaryColor);
        Theme.SetSecondaryColor(secondaryColor);

        UpdatePrimaryLabelTheme();
        UpdateSecondaryLabelTheme();

        if (baseTheme is BaseTheme.Inherit) SyncWithOs = true;
        else if (baseTheme is BaseTheme.Dark) LightDark = true;

        UpdateLanguage();

        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void ToggleButtonLightDark_OnChecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    private void ToggleButtonLightDark_OnUnchecked(object sender, RoutedEventArgs e)
        => UpdateBaseTheme();

    #endregion

    #region Function

    private static Color GetNewColor(Color defaultColor, string title)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        configuration.Interface.Theme.BaseTheme = SyncWithOs ? EBaseTheme.Inherit :
            LightDark is false ? EBaseTheme.Light : EBaseTheme.Dark;

        ((BaseTheme)configuration.Interface.Theme.BaseTheme).ApplyBaseTheme();
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
        BackgroundPrimaryLightBrush = Theme.PrimaryLight.Color.ToSolidColorBrush();
        BackgroundPrimaryMindBrush = Theme.PrimaryMid.Color.ToSolidColorBrush();
        BackgroundPrimaryDarkBrush = Theme.PrimaryDark.Color.ToSolidColorBrush();
    }

    private void UpdateSecondaryLabelTheme()
    {
        BackgroundSecondaryLightBrush = Theme.SecondaryLight.Color.ToSolidColorBrush();
        BackgroundSecondaryMindBrush = Theme.SecondaryMid.Color.ToSolidColorBrush();
        BackgroundSecondaryDarkBrush = Theme.SecondaryDark.Color.ToSolidColorBrush();
    }

    #endregion
}