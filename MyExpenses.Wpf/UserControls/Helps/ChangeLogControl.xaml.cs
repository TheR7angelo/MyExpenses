using System.Windows;
using Microsoft.Web.WebView2.Core;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.UserControls.Helps.ChangeLogControl;
using MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class ChangeLogControl
{
    #region DependencyProperty

    public static readonly DependencyProperty TextBlockVersionProperty =
        DependencyProperty.Register(nameof(TextBlockVersion), typeof(string), typeof(ChangeLogControl),
            new PropertyMetadata(default(string)));

    public string TextBlockVersion
    {
        get => (string)GetValue(TextBlockVersionProperty);
        set => SetValue(TextBlockVersionProperty, value);
    }

    public static readonly DependencyProperty TextBlockNewVersionIsAvailableProperty =
        DependencyProperty.Register(nameof(TextBlockNewVersionIsAvailable), typeof(string), typeof(ChangeLogControl),
            new PropertyMetadata(default(string)));

    public string TextBlockNewVersionIsAvailable
    {
        get => (string)GetValue(TextBlockNewVersionIsAvailableProperty);
        set => SetValue(TextBlockNewVersionIsAvailableProperty, value);
    }

    #endregion

    public bool IsNeedUpdate { get; }
    private bool InitialNavigation { get; set; } = true;

    public ChangeLogControl()
    {
        UpdateLanguage();

        // IsNeedUpdate = AutoUpdaterGitHub.NeedUpdate();
        IsNeedUpdate = false;

        InitializeComponent();

        InitializeAsync();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void WebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (InitialNavigation)
        {
            InitialNavigation = false;
            return;
        }

        e.Cancel = true;
        e.Uri.StartProcess();
    }

    #endregion

    #region Function

    private async void InitializeAsync()
    {
        await WebView2.EnsureCoreWebView2Async();

        var url = AutoUpdaterGitHub.HtmlFilePath;
        WebView2.CoreWebView2.Navigate(url);
    }

    private void UpdateLanguage()
    {
        TextBlockVersion = ChangeLogControlResources.TextBlockVersion;
        TextBlockNewVersionIsAvailable = ChangeLogControlResources.TextBlockNewVersionIsAvailable;
    }

    #endregion
}