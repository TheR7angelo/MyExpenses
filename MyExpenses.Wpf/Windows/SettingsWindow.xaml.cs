using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Windows.SettingsWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class SettingsWindow
{
    #region Resx

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(SettingsWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    public static readonly DependencyProperty TreeViewItemAppearanceHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemAppearanceHeader), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemAppearanceHeader
    {
        get => (string)GetValue(TreeViewItemAppearanceHeaderProperty);
        set => SetValue(TreeViewItemAppearanceHeaderProperty, value);
    }

    public static readonly DependencyProperty TreeViewItemLanguageHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemLanguageHeader), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemLanguageHeader
    {
        get => (string)GetValue(TreeViewItemLanguageHeaderProperty);
        set => SetValue(TreeViewItemLanguageHeaderProperty, value);
    }

    public static readonly DependencyProperty ButtonSaveContentProperty =
        DependencyProperty.Register(nameof(ButtonSaveContent), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string ButtonSaveContent
    {
        get => (string)GetValue(ButtonSaveContentProperty);
        set => SetValue(ButtonSaveContentProperty, value);
    }

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    #endregion

    public SettingsWindow()
    {
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    private void UpdateLanguage()
    {
        TitleWindow = SettingsWindowResources.TitleWindow;

        TreeViewItemAppearanceHeader = SettingsWindowResources.TreeViewItemAppearanceHeader;
        TreeViewItemLanguageHeader = SettingsWindowResources.TreeViewItemLanguageHeader;

        ButtonSaveContent = SettingsWindowResources.ButtonSaveContent;
        ButtonCancelContent = SettingsWindowResources.ButtonCancelContent;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => Close();

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (TabControl.SelectedItem is not TabItem tabItem) return;

        var configuration = Config.Configuration;
        if (tabItem.Header.Equals(ItemAppearance.Header))
        {
            var primaryColor = AppearanceControl.Theme.PrimaryMid.Color;
            var secondaryColor = AppearanceControl.Theme.SecondaryMid.Color;

            configuration.Interface.Theme.HexadecimalCodePrimaryColor = primaryColor.ToHexadecimal();
            configuration.Interface.Theme.HexadecimalCodeSecondaryColor = secondaryColor.ToHexadecimal();

            configuration.WriteConfiguration();

            App.LoadInterfaceTheme(configuration.Interface.Theme);

            Interface.OnThemeChanged(this, new ConfigurationThemeChangedEventArgs(configuration.Interface.Theme));
        }

        if (tabItem.Header.Equals(ItemLanguage.Header))
        {
            var cultureInfoCode = LanguageControl.CultureInfoSelected.Name;

            configuration.Interface.Language = cultureInfoCode;
            configuration.Interface.Clock.Is24Hours = LanguageControl.Is24Hours;

            configuration.WriteConfiguration();

            App.LoadInterfaceLanguage(cultureInfoCode);
            DbContextHelper.UpdateDbLanguage();

            Interface.OnLanguageChanged(this, new ConfigurationLanguageChangedEventArgs(cultureInfoCode));
        }

        Configuration.OnConfigurationChanged(this, new ConfigurationChangedEventArgs(configuration));
    }

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = (TreeViewItem)sender;
        if (treeViewItem.Header is not string header) return;

        var tabItem = TabControl.FindTabItemByHeader(header);
        if (tabItem is not null) tabItem.IsSelected = true;
    }

    #endregion
}