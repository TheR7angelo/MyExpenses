﻿using System.IO;
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

    private void WebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        var uri = e.Uri;

        if (uri.StartsWith("file:///"))
        {
            return;
        }

        e.Cancel = true;
        uri.StartProcess();
    }

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

    private async void ButtonUpdateNow_OnClick(object sender, RoutedEventArgs e)
        => await UpdateApplication();

    /// <summary>
    /// Handles the click event for the "Call Back Later" button. Displays a window
    /// to determine a new callback time and updates the configuration accordingly.
    /// The method delays the operation until the selected time has passed.
    /// If "Download Later Now" is chosen, the application is updated immediately.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the selected callback time is out of the predefined range.
    /// </exception>
    private async void ButtonCallBackLater_OnClick(object sender, RoutedEventArgs e)
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

    private async Task UpdateApplication()
    {
        var tempDirectory = Path.GetFullPath("Auto Update");
        Directory.CreateDirectory(tempDirectory);

        var assetTest = LastRelease.Assets![5]!;
        var pathTest = Path.Join(tempDirectory, assetTest.Name);

        var progressBarWindow = new ProgressBarWindow();
        progressBarWindow.Show();

        // await progressBarWindow.StartProgressBarDownload(assetTest.BrowserDownloadUrl!, pathTest, true);
        //TODO test with 10GB file download
        await progressBarWindow.StartProgressBarDownload("https://ash-speed.hetzner.com/10GB.bin", pathTest, true);

        pathTest.StartProcess();
        Application.Current.Shutdown();
    }
}