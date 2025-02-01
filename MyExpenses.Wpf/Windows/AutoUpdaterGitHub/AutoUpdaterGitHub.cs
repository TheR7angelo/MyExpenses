using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using MyExpenses.IO.MarkDown;
using MyExpenses.Models.WebApi.Github.Soft;
using MyExpenses.SharedUtils.Utils;
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
    public static string HtmlFilePath => Path.Join(VersioningPath, Path.ChangeExtension(FileName, ".html"));
    public static Release? LastRelease { get; private set; }

    private const string ApplicationOwner = "TheR7angelo";
    private const string ApplicationRepository = "MyExpenses";

    static AutoUpdaterGitHub()
    {
        Directory.CreateDirectory(VersioningPath);
    }

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
            await Task.Factory.StartNew(async () =>
            {
                var now = DateTime.Now;
                var configuration = Config.Configuration;
                var callBackLaterTime = configuration.System.CallBackLaterTime;

                if (callBackLaterTime > now)
                {
                    var delay = (DateTime)callBackLaterTime - now;
                    Log.Information("Delaying update check for {Delay} (hh:mm:ss) due to callback later time", delay.ToString(@"hh\:mm\:ss"));

                    await Task.Delay(delay, App.CancellationTokenSource.Token);
                }

                App.CancellationTokenSource.Token.ThrowIfCancellationRequested();

                var needUpdate = await CheckUpdateGitHubAsync();

                if (!needUpdate) return;

                Application.Current.Dispatcher.Invoke(Initialize);

            }, App.CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }, App.CancellationTokenSource.Token);
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
            Log.Information($"Fetched release notes from GitHub with owner: \"{ApplicationOwner}\" and repository: \"{ApplicationRepository}\"");
            var releasesNotesTmp = await GitHubClient.GetReleaseNotes(ApplicationOwner, ApplicationRepository);
            if (releasesNotesTmp is not null && releasesNotesTmp.Count > 0)
            {
                releasesNotes = releasesNotesTmp;
                UpdateReleaseNotesFiles(releasesNotes);
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to fetch release notes from GitHub");
        }

        LastRelease = releasesNotes?.OrderByDescending(s => s.Version).FirstOrDefault();

        return NeedUpdate();
    }

    /// <summary>
    /// Gets the default release notes from the JSON file.
    /// </summary>
    /// <returns>A list of Release objects representing the release notes.</returns>
    private static List<Release>? GetDefaultReleaseNotes()
    {
        if (!File.Exists(JsonFilePath)) return null;

        Log.Information("Loading default release notes from JSON file");
        var json = File.ReadAllText(JsonFilePath);
        var releasesNotes = json.ToObject<List<Release>>()!;

        if (!File.Exists(HtmlFilePath)) releasesNotes.UpdateReleaseNotesFiles();

        return releasesNotes;
    }

    /// <summary>
    /// Represents a static class that provides methods to perform automatic updates using GitHub as the source.
    /// </summary>
    private static void Initialize()
    {
        Log.Information("Initializing update dialog");

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var autoUpdaterGitHubWindow = new AutoUpdaterGitHubWindow(HtmlFilePath, LastRelease!);

        Log.Information("Showing update dialog");
        autoUpdaterGitHubWindow.ShowDialog();
    }

    /// <summary>
    /// Determines if an update is necessary by comparing the latest GitHub release with the current assembly version.
    /// </summary>
    /// <returns>True if an update is necessary, false otherwise.</returns>
    public static bool NeedUpdate()
    {
        var currentAssembly = Assembly.GetExecutingAssembly().GetName();
        var versionCompare = LastRelease is null
            ? currentAssembly.Version
            : LastRelease.Version;

        var result = versionCompare > currentAssembly.Version;
        Log.Information("Comparing versions: Local - {LocalVersion}, GitHub - {GitHubVersion}, Update Needed: {UpdateNeeded}", currentAssembly.Version, LastRelease?.Version, result);

        return result;
    }

    /// <summary>
    /// Updates the release notes files by converting the release notes to JSON, Markdown, and HTML formats
    /// and writing them to the corresponding files.
    /// </summary>
    /// <param name="releasesNotes">The list of release notes to update</param>
    /// <returns>The updated list of release notes</returns>
    private static void UpdateReleaseNotesFiles(this List<Release> releasesNotes)
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
        File.WriteAllText(JsonFilePath, json);

        Log.Information("Writing release notes to HTML file");
        File.WriteAllText(HtmlFilePath, htmlContent);
    }

    /// <summary>
    /// Identifies and returns the appropriate asset for the current system based on its architecture.
    /// </summary>
    /// <param name="assets">A collection of assets to search through.</param>
    /// <returns>The asset that matches the system's architecture and file extension, or null if no match is found.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the system architecture is not recognized.</exception>
    public static Asset? GetAssetForThisSystem(this IEnumerable<Asset> assets)
    {
        var architectureSuffix = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            _ => throw new ArgumentOutOfRangeException(null,
                @$"The system architecture '{RuntimeInformation.OSArchitecture}' is not recognized or supported")
        };

        Span<string> possibleExtensions = [".msi", ".exe"];
        foreach (var asset in assets)
        {
            if (asset.Name is null)
            {
                continue;
            }

            foreach (var ext in possibleExtensions)
            {
                if (asset.Name.EndsWith($"{architectureSuffix}{ext}", StringComparison.OrdinalIgnoreCase))
                {
                    return asset;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Updates the application by downloading the specified asset
    /// and executing the necessary steps for installation.
    /// </summary>
    /// <param name="asset">The asset containing the information required for the update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task UpdateApplication(this Asset asset)
    {
        var tempDirectory = Path.GetFullPath("Auto Update");
        Directory.CreateDirectory(tempDirectory);

        var savePath = Path.Join(tempDirectory, asset.Name);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var progressBarWindow = new ProgressBarWindow();
        progressBarWindow.Show();

        await progressBarWindow.StartProgressBarDownload(asset.BrowserDownloadUrl!, savePath, true);

        savePath.StartProcess();
        Application.Current.Shutdown();
    }
}