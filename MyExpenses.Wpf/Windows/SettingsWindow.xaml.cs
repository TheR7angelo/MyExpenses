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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(SettingsWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TreeViewItemAppearanceHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemAppearanceHeader), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemAppearanceHeader
    {
        get => (string)GetValue(TreeViewItemAppearanceHeaderProperty);
        set => SetValue(TreeViewItemAppearanceHeaderProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TreeViewItemLanguageHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemLanguageHeader), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemLanguageHeader
    {
        get => (string)GetValue(TreeViewItemLanguageHeaderProperty);
        set => SetValue(TreeViewItemLanguageHeaderProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonSaveContentProperty =
        DependencyProperty.Register(nameof(ButtonSaveContent), typeof(string), typeof(SettingsWindow),
            new PropertyMetadata(default(string)));

    public string ButtonSaveContent
    {
        get => (string)GetValue(ButtonSaveContentProperty);
        set => SetValue(ButtonSaveContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void UpdateLanguage()
    {
        TitleWindow = SettingsWindowResources.TitleWindow;

        TreeViewItemAppearanceHeader = SettingsWindowResources.TreeViewItemAppearanceHeader;
        TreeViewItemLanguageHeader = SettingsWindowResources.TreeViewItemLanguageHeader;

        ButtonSaveContent = SettingsWindowResources.ButtonSaveContent;
        ButtonCancelContent = SettingsWindowResources.ButtonCancelContent;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => Close();

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        if (TabControl.SelectedItem is not TabItem tabItem) return;

        var dictionary = new Dictionary<object, Func<Task>>
        {
            { ItemAppearance.Header, UpdateAppearanceSettings },
            { ItemLanguage.Header, UpdateLanguageSettings },
        };

        if (!dictionary.TryGetValue(tabItem.Header, out var func)) return;

        func.Invoke();
        Configuration.OnConfigurationChanged();
    }

    private Task UpdateLanguageSettings()
    {
        var cultureInfoCode = LanguageControl.CultureInfoSelected.Name;

        Config.Configuration.Interface.Language = cultureInfoCode;
        Config.Configuration.Interface.Clock.Is24Hours = LanguageControl.Is24Hours;

        Config.Configuration.WriteConfiguration();

        App.LoadInterfaceLanguage(cultureInfoCode);
        DbContextHelper.UpdateDbLanguage();

        Interface.OnLanguageChanged();

        return Task.CompletedTask;
    }

    private Task UpdateAppearanceSettings()
    {
        var primaryColor = AppearanceControl.Theme.PrimaryMid.Color;
        var secondaryColor = AppearanceControl.Theme.SecondaryMid.Color;

        Config.Configuration.Interface.Theme.HexadecimalCodePrimaryColor = primaryColor.ToHexadecimal();
        Config.Configuration.Interface.Theme.HexadecimalCodeSecondaryColor = secondaryColor.ToHexadecimal();

        Config.Configuration.WriteConfiguration();

        App.LoadInterfaceTheme(Config.Configuration.Interface.Theme);

        Interface.OnThemeChanged();

        return Task.CompletedTask;
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