using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Wpf.Resources.Resx.Windows.HelpsWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class HelpsWindow
{
    #region Resx

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(HelpsWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TreeViewItemItemVersionHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemItemVersionHeader), typeof(string), typeof(HelpsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemItemVersionHeader
    {
        get => (string)GetValue(TreeViewItemItemVersionHeaderProperty);
        set => SetValue(TreeViewItemItemVersionHeaderProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TreeViewItemItemChangelogHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemItemChangelogHeader), typeof(string), typeof(HelpsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemItemChangelogHeader
    {
        get => (string)GetValue(TreeViewItemItemChangelogHeaderProperty);
        set => SetValue(TreeViewItemItemChangelogHeaderProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TreeViewItemItemHowToUseHeaderProperty =
        DependencyProperty.Register(nameof(TreeViewItemItemHowToUseHeader), typeof(string), typeof(HelpsWindow),
            new PropertyMetadata(default(string)));

    public string TreeViewItemItemHowToUseHeader
    {
        get => (string)GetValue(TreeViewItemItemHowToUseHeaderProperty);
        set => SetValue(TreeViewItemItemHowToUseHeaderProperty, value);
    }

    #endregion

    public HelpsWindow()
    {
        UpdateLanguage();

        InitializeComponent();

        this.SetWindowCornerPreference();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var treeViewItem = (TreeViewItem)sender;
        if (treeViewItem.Header is not string header) return;

        var tabItem = TabControl.FindTabItemByHeader(header);
        if (tabItem is not null) tabItem.IsSelected = true;
    }

    #endregion

    #region Function

    private void UpdateLanguage()
    {
        TitleWindow = HelpsWindowResources.TitleWindow;

        TreeViewItemItemVersionHeader = HelpsWindowResources.TreeViewItemItemVersionHeader;
        TreeViewItemItemChangelogHeader = HelpsWindowResources.TreeViewItemItemChangelogHeader;
        TreeViewItemItemHowToUseHeader = HelpsWindowResources.TreeViewItemItemHowToUseHeader;
    }

    #endregion
}