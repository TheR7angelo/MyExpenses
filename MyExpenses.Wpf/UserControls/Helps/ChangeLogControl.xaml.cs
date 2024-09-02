using System.Windows;
using Microsoft.Web.WebView2.Core;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.WebApi.Github.Soft;
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

    public static readonly DependencyProperty ButtonUpdateContentProperty =
        DependencyProperty.Register(nameof(ButtonUpdateContent), typeof(string), typeof(ChangeLogControl),
            new PropertyMetadata(default(string)));

    public string ButtonUpdateContent
    {
        get => (string)GetValue(ButtonUpdateContentProperty);
        set => SetValue(ButtonUpdateContentProperty, value);
    }

    #endregion

    public bool IsNeedUpdate { get; }
    private bool InitialNavigation { get; set; } = true;

    public ChangeLogControl()
    {
        UpdateLanguage();

        IsNeedUpdate = AutoUpdaterGitHub.NeedUpdate();

        InitializeComponent();

        InitializeAsync();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    //TODO test with 10GB file download
    private async void ButtonUpdate_OnClick(object sender, RoutedEventArgs e)
    {
        var assetTest = new Asset
        {
            Name = "10GB.bin",
            BrowserDownloadUrl = "https://ash-speed.hetzner.com/10GB.bin"
        };

        await assetTest.UpdateApplication();
    }

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
        ButtonUpdateContent = ChangeLogControlResources.ButtonUpdateContent;
    }

    #endregion
}