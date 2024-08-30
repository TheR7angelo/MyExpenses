using System.IO;
using System.Reflection;
using System.Windows;
using MyExpenses.IO.MarkDown;
using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.Utils;
using MyExpenses.WebApi.GitHub;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public static class AutoUpdaterGitHub
{
    private static string ResourcePath => Path.GetFullPath("Resources");
    private static string VersioningPath => Path.Join(ResourcePath, "Versioning");
    private static string FileName => "version";
    private static string JsonFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".json"));
    private static string HtmlFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".html"));

    // Juste for testing
    // TODO adjust
    private const string ApplicationOwner = "microsoft";
    // Juste for testing
    // TODO adjust
    private const string ApplicationRepository = "PowerToys";

    /// <summary>
    /// Runs a task asynchronously
    /// to check if an update is necessary
    /// by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    public static void CheckUpdateGitHub()
    {
        Task.Run(async () =>
        {
            var needUpdate = await CheckUpdateGitHubAsync();

            if (!needUpdate) return;

            Application.Current.Dispatcher.Invoke(Initialize);
        });
    }

    private static async Task<bool> CheckUpdateGitHubAsync()
    {
        var releasesNotes = GetDefaultReleaseNotes();
        try
        {
            using var gitHubClient = new GitHubClient();
            var releasesNotesTmp = await gitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotesTmp is not null)
            {
                releasesNotes = releasesNotesTmp;
                string background = null!;
                string foreground = null!;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    background = Utils.Resources.GetMaterialDesignPaperColorHexadecimalWithoutAlpha();
                    foreground = Utils.Resources.GetMaterialDesignBodyColorHexadecimalWithoutAlpha();
                });

                var json = releasesNotesTmp.ToJson();
                var markDown = releasesNotesTmp.ToMarkDown();
                var htmlContent = markDown.ToHtml(background, foreground);

                await File.WriteAllTextAsync(JsonFilePath, json);
                await File.WriteAllTextAsync(HtmlFilePath, htmlContent);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        var lastRelease = releasesNotes.OrderByDescending(s => s.PublishedAt).First();
        return lastRelease.NeedUpdate();
    }

    /// <summary>
    /// Determines if an update is necessary by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    /// <param name="release">The latest release information from GitHub.</param>
    /// <returns>True if an update is necessary, false otherwise.</returns>
    private static bool NeedUpdate(this Release release)
    {
        var currentAssembly = Assembly.GetExecutingAssembly().GetName();

        var tagName = release.TagName;
        if (string.IsNullOrEmpty(tagName)) tagName = currentAssembly.Version!.ToString();
        tagName = tagName.Replace("v", string.Empty);

        var githubLastVersion = new Version(tagName);

        return githubLastVersion > currentAssembly.Version;
    }

    /// <summary>
    /// Represents a static class that provides methods to perform automatic updates using GitHub as the source.
    /// </summary>
    private static void Initialize()
    {
        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(HtmlFilePath)
        {
            Owner = Application.Current.MainWindow,
        };
        autoUpdaterGitHubWindow.ShowDialog();
    }

    /// <summary>
    /// Gets the default release notes from the JSON file.
    /// </summary>
    /// <returns>A list of Release objects representing the release notes.</returns>
    private static List<Release> GetDefaultReleaseNotes()
    {
        var json = File.ReadAllText(JsonFilePath);
        var releasesNotes = json.ToObject<List<Release>>()!;

        return releasesNotes;
    }
}