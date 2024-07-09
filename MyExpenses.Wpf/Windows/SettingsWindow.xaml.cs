using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class SettingsWindow
{
    public static readonly DependencyProperty BackgroundPrimaryMindBrushProperty = DependencyProperty.Register(
        nameof(BackgroundPrimaryMindBrush),
        typeof(Brush), typeof(SettingsWindow), new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BackgroundPrimaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryLightBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BackgroundPrimaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundPrimaryDarkBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BackgroundSecondaryLightBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryLightBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BackgroundSecondaryMindBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryMindBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty BackgroundSecondaryDarkBrushProperty =
        DependencyProperty.Register(nameof(BackgroundSecondaryDarkBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundPrimaryMindBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryMindBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundPrimaryLightBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryLightBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundPrimaryDarkBrushProperty =
        DependencyProperty.Register(nameof(ForegroundPrimaryDarkBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundSecondaryLightBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryLightBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundSecondaryMindBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryMindBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public static readonly DependencyProperty ForegroundSecondaryDarkBrushProperty =
        DependencyProperty.Register(nameof(ForegroundSecondaryDarkBrush), typeof(Brush), typeof(SettingsWindow),
            new PropertyMetadata(default(Brush)));

    public SettingsWindow()
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();

        BackgroundPrimaryLightBrush = new SolidColorBrush(theme.PrimaryLight.Color);
        BackgroundPrimaryMindBrush = new SolidColorBrush(theme.PrimaryMid.Color);
        BackgroundPrimaryDarkBrush = new SolidColorBrush(theme.PrimaryDark.Color);

        BackgroundSecondaryLightBrush = new SolidColorBrush(theme.SecondaryLight.Color);
        BackgroundSecondaryMindBrush = new SolidColorBrush(theme.SecondaryMid.Color);
        BackgroundSecondaryDarkBrush = new SolidColorBrush(theme.SecondaryDark.Color);

        ForegroundPrimaryLightBrush = new SolidColorBrush(theme.PrimaryLight.Color.ContrastingForegroundColor());
        ForegroundPrimaryMindBrush = new SolidColorBrush(theme.PrimaryMid.Color.ContrastingForegroundColor());
        ForegroundPrimaryDarkBrush = new SolidColorBrush(theme.PrimaryDark.Color.ContrastingForegroundColor());

        ForegroundSecondaryLightBrush = new SolidColorBrush(theme.SecondaryLight.Color.ContrastingForegroundColor());
        ForegroundSecondaryMindBrush = new SolidColorBrush(theme.SecondaryMid.Color.ContrastingForegroundColor());
        ForegroundSecondaryDarkBrush = new SolidColorBrush(theme.SecondaryDark.Color.ContrastingForegroundColor());

        InitializeComponent();
    }

    public Brush BackgroundPrimaryMindBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryMindBrushProperty);
        set => SetValue(BackgroundPrimaryMindBrushProperty, value);
    }

    public Brush BackgroundPrimaryLightBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryLightBrushProperty);
        set => SetValue(BackgroundPrimaryLightBrushProperty, value);
    }

    public Brush BackgroundPrimaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundPrimaryDarkBrushProperty);
        set => SetValue(BackgroundPrimaryDarkBrushProperty, value);
    }

    public Brush BackgroundSecondaryLightBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryLightBrushProperty);
        set => SetValue(BackgroundSecondaryLightBrushProperty, value);
    }

    public Brush BackgroundSecondaryMindBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryMindBrushProperty);
        set => SetValue(BackgroundSecondaryMindBrushProperty, value);
    }

    public Brush BackgroundSecondaryDarkBrush
    {
        get => (Brush)GetValue(BackgroundSecondaryDarkBrushProperty);
        set => SetValue(BackgroundSecondaryDarkBrushProperty, value);
    }

    public Brush ForegroundPrimaryMindBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryMindBrushProperty);
        set => SetValue(ForegroundPrimaryMindBrushProperty, value);
    }

    public Brush ForegroundPrimaryLightBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryLightBrushProperty);
        set => SetValue(ForegroundPrimaryLightBrushProperty, value);
    }

    public Brush ForegroundPrimaryDarkBrush
    {
        get => (Brush)GetValue(ForegroundPrimaryDarkBrushProperty);
        set => SetValue(ForegroundPrimaryDarkBrushProperty, value);
    }

    public Brush ForegroundSecondaryLightBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryLightBrushProperty);
        set => SetValue(ForegroundSecondaryLightBrushProperty, value);
    }

    public Brush ForegroundSecondaryMindBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryMindBrushProperty);
        set => SetValue(ForegroundSecondaryMindBrushProperty, value);
    }

    public Brush ForegroundSecondaryDarkBrush
    {
        get => (Brush)GetValue(ForegroundSecondaryDarkBrushProperty);
        set => SetValue(ForegroundSecondaryDarkBrushProperty, value);
    }

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = (TreeViewItem)sender;
        if (treeViewItem.Header is not string header) return;

        var tabItem = FindTabItemByHeader(TabControl, header);
        if (tabItem is not null) tabItem.IsSelected = true;
    }

    private static TabItem? FindTabItemByHeader(TabControl tabControl, string header)
    {
        var children = tabControl.FindVisualChildren<TabItem>();
        return children.FirstOrDefault(child => child.Header.Equals(header));
    }
}