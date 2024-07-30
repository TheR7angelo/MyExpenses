using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Utils;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class SettingsWindow
{
    //TODO add language
    public SettingsWindow()
    {
        InitializeComponent();

        this.SetWindowCornerPreference();
    }

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
            configuration.WriteConfiguration();

            //TODO add listener to update all text ...
            App.LoadInterfaceLanguage(cultureInfoCode);

            Interface.OnLanguageChanged(this, new ConfigurationLanguageChangedEventArgs(cultureInfoCode));
        }

        Configuration.OnConfigurationChanged(this, new ConfigurationChangedEventArgs(configuration));
    }

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = (TreeViewItem)sender;
        if (treeViewItem.Header is not string header) return;

        var tabItem = FindTabItemByHeader(TabControl, header);
        if (tabItem is not null) tabItem.IsSelected = true;
    }

    #endregion

    #region Function

    private static TabItem? FindTabItemByHeader(TabControl tabControl, string header)
    {
        var children = tabControl.FindVisualChildren<TabItem>();
        return children.FirstOrDefault(child => child.Header.Equals(header));
    }

    #endregion
}