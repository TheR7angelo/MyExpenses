using Microsoft.Web.WebView2.Core;
using MyExpenses.Utils;
using MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

namespace MyExpenses.Wpf.UserControls.Helps;

public partial class ChangeLogControl
{
    public ChangeLogControl()
    {
        InitializeComponent();

        InitializeAsync();
    }

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

    private async void InitializeAsync()
    {
        await WebView2.EnsureCoreWebView2Async();

        var url = AutoUpdaterGitHub.HtmlFilePath;
        WebView2.CoreWebView2.Navigate(url);
    }
}