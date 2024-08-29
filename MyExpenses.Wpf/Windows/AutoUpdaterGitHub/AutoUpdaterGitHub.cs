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
        Task.Run(async () => { await CheckUpdateGitHubAsync(); });
    }

    private static async Task CheckUpdateGitHubAsync()
    {
        using var gitHubClient = new GitHubClient();

        try
        {
            var releasesNotes = await gitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotes is null) return; // Juste for testing


        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void Initialize()
    {
        const string releasesUrl = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\Tests\MyExpenses.IO.Test\bin\Debug\net8.0\test.html";

        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(releasesUrl)
        {
            Owner = Application.Current.MainWindow,
        };
        autoUpdaterGitHubWindow.ShowDialog();
    }
}