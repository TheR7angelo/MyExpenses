using System.Reflection;
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

    public static readonly DependencyProperty TextBlockNewVersionIsAvailableProperty =
        DependencyProperty.Register(nameof(TextBlockNewVersionIsAvailable), typeof(string),
            typeof(AutoUpdaterGitHubWindow), new PropertyMetadata(default(string)));

    public string TextBlockNewVersionIsAvailable
    {
        get => (string)GetValue(TextBlockNewVersionIsAvailableProperty);
        set => SetValue(TextBlockNewVersionIsAvailableProperty, value);
    }

    public static readonly DependencyProperty TextBlockNewVersionIsAvailableParagraphProperty =
        DependencyProperty.Register(nameof(TextBlockNewVersionIsAvailableParagraph), typeof(string),
            typeof(AutoUpdaterGitHubWindow), new PropertyMetadata(default(string)));

    public string TextBlockNewVersionIsAvailableParagraph
    {
        get => (string)GetValue(TextBlockNewVersionIsAvailableParagraphProperty);
        set => SetValue(TextBlockNewVersionIsAvailableParagraphProperty, value);
    }

    public static readonly DependencyProperty TextBlockVersionNoteProperty =
        DependencyProperty.Register(nameof(TextBlockVersionNote), typeof(string), typeof(AutoUpdaterGitHubWindow),
            new PropertyMetadata(default(string)));

    public string TextBlockVersionNote
    {
        get => (string)GetValue(TextBlockVersionNoteProperty);
        set => SetValue(TextBlockVersionNoteProperty, value);
    }

    public static readonly DependencyProperty ButtonCallBackLaterContentProperty =
        DependencyProperty.Register(nameof(ButtonCallBackLaterContent), typeof(string), typeof(AutoUpdaterGitHubWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCallBackLaterContent
    {
        get => (string)GetValue(ButtonCallBackLaterContentProperty);
        set => SetValue(ButtonCallBackLaterContentProperty, value);
    }

    public static readonly DependencyProperty ButtonUpdateNowContentProperty =
        DependencyProperty.Register(nameof(ButtonUpdateNowContent), typeof(string), typeof(AutoUpdaterGitHubWindow),
            new PropertyMetadata(default(string)));

    public string ButtonUpdateNowContent
    {
        get => (string)GetValue(ButtonUpdateNowContentProperty);
        set => SetValue(ButtonUpdateNowContentProperty, value);
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
        var assembly = Assembly.GetExecutingAssembly().GetName();

        TitleWindow = string.Format(AutoUpdaterGitHubWindowResources.TitleWindow, assembly.Name, LastRelease.Version);
        TextBlockNewVersionIsAvailable = string.Format(AutoUpdaterGitHubWindowResources.TextBlockNewVersionIsAvailable, assembly.Name);
        TextBlockNewVersionIsAvailableParagraph = string.Format(AutoUpdaterGitHubWindowResources.TextBlockNewVersionIsAvailableParagraph, assembly.Name, LastRelease.Version, assembly.Version, Environment.NewLine);
        TextBlockVersionNote = AutoUpdaterGitHubWindowResources.TextBlockVersionNote;

        ButtonCallBackLaterContent = AutoUpdaterGitHubWindowResources.ButtonCallBackLaterContent;
        ButtonUpdateNowContent = AutoUpdaterGitHubWindowResources.ButtonUpdateNowContent;
    }

    #endregion
}