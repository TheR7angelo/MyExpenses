using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public partial class AutoUpdaterGitHubWindow
{
    public Release? LastRelease { get; init; }

    public AutoUpdaterGitHubWindow(string releasesUrl)
    {
        InitializeComponent();
        InitializeAsync(releasesUrl);

        this.SetWindowCornerPreference();
    }

    private async void InitializeAsync(string releasesUrl)
    {
        await WebView2.EnsureCoreWebView2Async();

        WebView2.CoreWebView2.Navigate(releasesUrl);
    }
}