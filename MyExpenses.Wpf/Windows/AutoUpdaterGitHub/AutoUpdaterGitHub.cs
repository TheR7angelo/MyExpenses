using System.IO;
using System.Windows;
using MyExpenses.IO.MarkDown;
using MyExpenses.WebApi.GitHub;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public static class AutoUpdaterGitHub
{
    private static string ResourcePath => Path.GetFullPath("Resources");
    private static string VersioningPath => Path.Join(ResourcePath, "Versioning");
    private static string FileName => "version";
    private static string MarkDownFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".md"));
    private static string HtmlFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".html"));

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

            Application.Current.Dispatcher.Invoke(Initialize);
        });
    }

    //TODO work
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

            await File.WriteAllTextAsync(MarkDownFilePath, markDown);
            await File.WriteAllTextAsync(HtmlFilePath, htmlContent);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            return false;
        }
    }

    private static void Initialize()
    {
        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(HtmlFilePath)
        {
            Owner = Application.Current.MainWindow,
        };
        autoUpdaterGitHubWindow.ShowDialog();
    }
}