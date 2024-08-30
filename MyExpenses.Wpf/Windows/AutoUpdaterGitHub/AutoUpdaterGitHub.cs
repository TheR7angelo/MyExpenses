using System.IO;
using System.Reflection;
using System.Windows;
using MyExpenses.IO.MarkDown;
using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.Utils;
using MyExpenses.WebApi.GitHub;
using Serilog;

namespace MyExpenses.Wpf.Windows.AutoUpdaterGitHub;

public static class AutoUpdaterGitHub
{
    private static string ResourcePath => Path.GetFullPath("Resources");
    private static string VersioningPath => Path.Join(ResourcePath, "Versioning");
    private static string FileName => "version";
    private static string JsonFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".json"));
    private static string HtmlFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".html"));
    private static Release? LastRelease { get; set; }

    private const string ApplicationOwner = "TheR7angelo";
    private const string ApplicationRepository = "MyExpenses";

    /// <summary>
    /// Runs a task asynchronously
    /// to check if an update is necessary
    /// by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    public static void CheckUpdateGitHub()
    {
        Log.Information("Starting update check");
        Task.Run(async () =>
        {
            var needUpdate = await CheckUpdateGitHubAsync();

            if (!needUpdate) return;

            Application.Current.Dispatcher.Invoke(Initialize);
        });
    }

    /// <summary>
    /// Runs a task asynchronously
    /// to check if an update is necessary
    /// by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    /// <returns>True if an update is necessary, false otherwise.</returns>
    private static async Task<bool> CheckUpdateGitHubAsync()
    {
        var releasesNotes = GetDefaultReleaseNotes();
        try
        {
            Log.Information("Fetched release notes from GitHub");
            using var gitHubClient = new GitHubClient();
            var releasesNotesTmp = await gitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotesTmp is not null && releasesNotesTmp.Count > 0)
            {
                releasesNotes = releasesNotesTmp;
                await UpdateReleaseNotesFiles(releasesNotes);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to fetch release notes from GitHub");
        }

        LastRelease = releasesNotes.OrderByDescending(s => s.PublishedAt).First();

        return LastRelease.NeedUpdate();
    }

    /// <summary>
    /// Gets the default release notes from the JSON file.
    /// </summary>
    /// <returns>A list of Release objects representing the release notes.</returns>
    private static List<Release> GetDefaultReleaseNotes()
    {
        Log.Information("Loading default release notes from JSON file");
        var json = File.ReadAllText(JsonFilePath);
        var releasesNotes = json.ToObject<List<Release>>()!;

        if (!File.Exists(HtmlFilePath)) _ = releasesNotes.UpdateReleaseNotesFiles();

        return releasesNotes;
    }

    /// <summary>
    /// Represents a static class that provides methods to perform automatic updates using GitHub as the source.
    /// </summary>
    private static void Initialize()
    {
        Log.Information("Initializing update dialog");
        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(HtmlFilePath, LastRelease!)
        {
            Owner = Application.Current.MainWindow
        };
        autoUpdaterGitHubWindow.ShowDialog();
    }

    /// <summary>
    /// Determines if an update is necessary by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    /// <param name="release">The latest release information from GitHub.</param>
    /// <returns>True if an update is necessary, false otherwise.</returns>
    private static bool NeedUpdate(this Release release)
    {
        var currentAssembly = Assembly.GetExecutingAssembly().GetName();

        var result = release.Version > currentAssembly.Version;
        Log.Information("Comparing versions: Local - {LocalVersion}, GitHub - {GitHubVersion}, Update Needed: {UpdateNeeded}", currentAssembly.Version, release.Version, result);

        return result;
    }

    /// <summary>
    /// Updates the release notes files by converting the release notes to JSON, Markdown, and HTML formats
    /// and writing them to the corresponding files.
    /// </summary>
    /// <param name="releasesNotes">The list of release notes to update</param>
    /// <returns>The updated list of release notes</returns>
    private static async Task UpdateReleaseNotesFiles(this List<Release> releasesNotes)
    {
        string background = null!;
        string foreground = null!;

        Application.Current.Dispatcher.Invoke(() =>
        {
            background = Utils.Resources.GetMaterialDesignPaperColorHexadecimalWithoutAlpha();
            foreground = Utils.Resources.GetMaterialDesignBodyColorHexadecimalWithoutAlpha();
        });

        var json = releasesNotes.ToJson();
        var markDown = releasesNotes.ToMarkDown();
        var htmlContent = markDown.ToHtml(background, foreground);

        Log.Information("Writing release notes to JSON file");
        await File.WriteAllTextAsync(JsonFilePath, json);

        Log.Information("Writing release notes to HTML file");
        await File.WriteAllTextAsync(HtmlFilePath, htmlContent);
    }
}