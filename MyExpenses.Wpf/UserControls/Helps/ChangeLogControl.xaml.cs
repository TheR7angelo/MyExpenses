using Microsoft.Web.WebView2.Core;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Utils;
using MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class ChangeLogControl
{
    private bool InitialNavigation { get; set; } = true;

    public ChangeLogControl()
    {
        UpdateLanguage();

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

    }

    #endregion
}