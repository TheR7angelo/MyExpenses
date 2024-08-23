using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.Windows.HelpsWindow;
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

    public static readonly DependencyProperty TreeViewItemItemVersionHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemItemVersionHeader), typeof(string), typeof(HelpsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemItemVersionHeader
    {
        get => (string)GetValue(TreeViewItemItemVersionHeaderProperty);
        set => SetValue(TreeViewItemItemVersionHeaderProperty, value);
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
        TitleWindow = HelpsWindowResources.TitleWindow;

        TreeViewItemItemVersionHeader = HelpsWindowResources.TreeViewItemItemVersionHeader;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #region Action

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = (TreeViewItem)sender;
        if (treeViewItem.Header is not string header) return;

        var tabItem = TabControl.FindTabItemByHeader(header);
        if (tabItem is not null) tabItem.IsSelected = true;
    }

    #endregion
}