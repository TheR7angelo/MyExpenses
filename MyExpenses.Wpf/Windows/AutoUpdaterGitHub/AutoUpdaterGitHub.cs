using System.Windows;
using MyExpenses.WebApi.GitHub;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public static class AutoUpdaterGitHub
{
    // Juste for testing
    private const string ApplicationOwner = "microsoft";
    // Juste for testing
    private const string ApplicationRepository = "PowerToys";

    public static void CheckUpdateGitHub()
    {
        Task.Run(async () =>
        {
            var needUpdate = await CheckUpdateGitHubAsync();

            if (!needUpdate) return;

            const string releasesUrl = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\Tests\MyExpenses.IO.Test\bin\Debug\net8.0\test.html";
            Application.Current.Dispatcher.Invoke(() => Initialize(releasesUrl));
        });
    }

    private static async Task<bool> CheckUpdateGitHubAsync()
    {
        using var gitHubClient = new GitHubClient();

        try
        {
            var releasesNotes = await gitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotes is null) return false; // Juste for testing

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return false;
        }
    }

    private static void Initialize(this string releasesUrl)
    {
        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(releasesUrl)
        {
            Owner = Application.Current.MainWindow,
        };
        autoUpdaterGitHubWindow.ShowDialog();
    }
}