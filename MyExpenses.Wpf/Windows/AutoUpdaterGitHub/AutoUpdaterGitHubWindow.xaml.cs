using System.Reflection;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.Models.Wpf.AutoUpdaterGitHub;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.Windows.AutoUpdaterGitHubWindow;
using MyExpenses.Wpf.Utils;
using Serilog;

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

    private bool InitialNavigation { get; set; } = true;

    private Release LastRelease { get; }

    public AutoUpdaterGitHubWindow(string releasesUrl, Release lastRelease)
    {
        LastRelease = lastRelease;

        UpdateLanguage();

        InitializeComponent();

        _ = InitializeAsync(releasesUrl);

        this.SetWindowCornerPreference();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCallBackLater_OnClick(object sender, RoutedEventArgs e)
        => _ = HandleButtonCallBackLater();

    private void ButtonUpdateNow_OnClick(object sender, RoutedEventArgs e)
        => _ = UpdateApplication();

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

    private async Task HandleButtonCallBackLater()
    {
                var callBackLaterWindow = new CallBackLaterWindow();
        var result = callBackLaterWindow.ShowDialog();

        if (result is false or null) Close();

        if (callBackLaterWindow.RadioButtonDownloadLaterNoIsChecked)
        {
            Log.Information("Download later now selected. Updating application now");
            await UpdateApplication();
            return;
        }

        var now = DateTime.Now;
        var newAsk = callBackLaterWindow.SelectedCallBackLaterTime switch
        {
            CallBackLaterTime.After30Minutes => now.AddMinutes(30),
            CallBackLaterTime.After12Hours => now.AddHours(12),
            CallBackLaterTime.After1Days => now.AddDays(1),
            CallBackLaterTime.After2Days => now.AddDays(2),
            CallBackLaterTime.After4Days => now.AddDays(4),
            CallBackLaterTime.After8Days => now.AddDays(8),
            CallBackLaterTime.After10Days => now.AddDays(10),
            _ => throw new ArgumentOutOfRangeException()
        };

        Log.Information("New callback time set to: {NewAsk} (hh:mm:ss)", newAsk.ToString(@"hh\:mm\:ss"));

        var configuration = Config.Configuration;
        configuration.System.CallBackLaterTime = newAsk;
        configuration.WriteConfiguration();

        Log.Information("Configuration updated with new callback time");

        Hide();

        try
        {
            Log.Information("Delaying operation for {Delay} (hh:mm:ss)", (newAsk - now).ToString(@"hh\:mm\:ss"));
            await Task.Delay(newAsk - now, App.CancellationTokenSource.Token);

            if (App.CancellationTokenSource.Token.IsCancellationRequested)
            {
                Log.Information("Operation cancelled");
                Close();
                return;
            }

            Log.Information("Resuming operation after delay");
            ShowDialog();
            Activate();
        }
        catch (TaskCanceledException)
        {
            Log.Information("Task was cancelled during delay");
            Close();
        }
    }

    private async Task InitializeAsync(string releasesUrl)
    {
        await WebView2.EnsureCoreWebView2Async();

        WebView2.CoreWebView2.Navigate(releasesUrl);
    }

    private static async Task UpdateApplication()
    {
        var lastRelease = AutoUpdaterGitHub.LastRelease!;
        var asset = lastRelease.Assets!.GetAssetForThisSystem();
        if (asset is null)
        {
            Log.Error("No asset found for this system");
            return;
        }

        // var assetTest = new Asset
        // {
        //     Name = "10GB.bin",
        //     BrowserDownloadUrl = "https://ash-speed.hetzner.com/10GB.bin"
        // };

        // await assetTest.UpdateApplication();
        await asset.UpdateApplication();
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