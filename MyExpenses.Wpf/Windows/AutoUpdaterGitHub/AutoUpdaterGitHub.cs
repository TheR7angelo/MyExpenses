using System.IO;
using System.Windows;
using MyExpenses.IO.MarkDown;
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

            string background = null!;
            string foreground = null!;

            Application.Current.Dispatcher.Invoke(() =>
            {
                background = Utils.Resources.GetMaterialDesignPaperColorHexadecimalWithoutAlpha();
                foreground = Utils.Resources.GetMaterialDesignBodyColorHexadecimalWithoutAlpha();
            });

            var markDown = releasesNotes.ToMarkDown();
            var htmlContent = markDown.ToHtml(background, foreground);

            var resourcePath = Path.GetFullPath("Resources");
            var versioningPath = Path.Join(resourcePath, "Versioning");

            var mdFilePath = Path.Join(versioningPath, "version.md");
            var htmlFilePath = Path.Join(versioningPath, "versioning.html");

            await File.WriteAllTextAsync(mdFilePath, markDown);
            await File.WriteAllTextAsync(htmlFilePath, htmlContent);

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