using System.IO;
using System.Text.RegularExpressions;
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
        var markDown = await File.ReadAllTextAsync(MarkDownFilePath);
        try
        {
            using var gitHubClient = new GitHubClient();
            var releasesNotes = await gitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotes is not null)
            {
                string background = null!;
                string foreground = null!;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    background = Utils.Resources.GetMaterialDesignPaperColorHexadecimalWithoutAlpha();
                    foreground = Utils.Resources.GetMaterialDesignBodyColorHexadecimalWithoutAlpha();
                });

                markDown = releasesNotes.ToMarkDown();
                var htmlContent = markDown.ToHtml(background, foreground);

                await File.WriteAllTextAsync(MarkDownFilePath, markDown);
                await File.WriteAllTextAsync(HtmlFilePath, htmlContent);

                const string pattern = "<!--\\s([\\s\\S]*?)-->\\s+___";
                var matches = Regex.Matches(markDown, pattern, RegexOptions.Multiline);

                foreach (Match match in matches)
                {
                    // Iterate over the groups starting from index 1 to skip the entire match (Group[0])
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        Group group = match.Groups[i];
                        var value = group.Value;
                        Console.WriteLine($"Group {i}: {value}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine(markDown); // Juste for testing
        return true;
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