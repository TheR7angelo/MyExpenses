using System.Windows;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.Wpf.Resources.Resx.Windows.AutoUpdaterGitHubWindow;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public partial class AutoUpdaterGitHubWindow
{
    #region DependencyProperty

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AutoUpdaterGitHubWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    private Release LastRelease { get; }

    public AutoUpdaterGitHubWindow(string releasesUrl, Release lastRelease)
    {
        LastRelease = lastRelease;

        UpdateLanguage();

        InitializeComponent();

        InitializeAsync(releasesUrl);

        this.SetWindowCornerPreference();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    #region Function

    private async void InitializeAsync(string releasesUrl)
    {
        await WebView2.EnsureCoreWebView2Async();

        WebView2.CoreWebView2.Navigate(releasesUrl);
    }

    private void UpdateLanguage()
    {
        TitleWindow = string.Format(AutoUpdaterGitHubWindowResources.TitleWindow, AutoUpdaterGitHub.ApplicationRepository, LastRelease.Version);
    }

    #endregion
}