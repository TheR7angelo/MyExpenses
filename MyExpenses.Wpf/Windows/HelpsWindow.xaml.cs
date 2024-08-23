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

public partial class HelpsWindow
{
    #region Resx

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(HelpsWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    public HelpsWindow()
    {
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    private void UpdateLanguage()
    {
        TitleWindow = SettingsWindowResources.TitleWindow;

    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #region Action

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

    private static void UpdateDbLanguage()
    {
        var newExistingDatabases = DbContextBackup.GetExistingDatabase();
        foreach (var newExistingDatabase in newExistingDatabases)
        {
            using var context = new DataBaseContext(newExistingDatabase.FilePath);
            context.UpdateAllDefaultValues();
            context.SaveChanges();
        }
    }

    #endregion
}