using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.WebApi;
using MyExpenses.Wpf.Resources.Resx.Windows.ProgressBarWindow;
using MyExpenses.Wpf.Utils;
using Timer = System.Timers.Timer;

namespace MyExpenses.Wpf.Windows;

public partial class ProgressBarWindow
{
    #region DependencyProperty

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty SpeedProgressProperty = DependencyProperty.Register(nameof(SpeedProgress),
        typeof(string), typeof(ProgressBarWindow), new PropertyMetadata(default(string)));

    public string SpeedProgress
    {
        get => (string)GetValue(SpeedProgressProperty);
        set => SetValue(SpeedProgressProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TimeLeftProgressProperty =
        DependencyProperty.Register(nameof(TimeLeftProgress), typeof(TimeSpan), typeof(ProgressBarWindow),
            new PropertyMetadata(TimeSpan.Zero));

    // ReSharper disable once HeapView.BoxingAllocation
    public TimeSpan TimeLeftProgress
    {
        get => (TimeSpan)GetValue(TimeLeftProgressProperty);
        set => SetValue(TimeLeftProgressProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TimeElapsedProperty = DependencyProperty.Register(nameof(TimeElapsed),
        typeof(TimeSpan), typeof(ProgressBarWindow), new PropertyMetadata(TimeSpan.Zero));

    // ReSharper disable once HeapView.BoxingAllocation
    public TimeSpan TimeElapsed
    {
        get => (TimeSpan)GetValue(TimeElapsedProperty);
        set => SetValue(TimeElapsedProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelTimeElapsedProperty =
        DependencyProperty.Register(nameof(LabelTimeElapsed), typeof(string), typeof(ProgressBarWindow),
            new PropertyMetadata(default(string)));

    public string LabelTimeElapsed
    {
        get => (string)GetValue(LabelTimeElapsedProperty);
        set => SetValue(LabelTimeElapsedProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelTimeLeftProperty = DependencyProperty.Register(nameof(LabelTimeLeft),
        typeof(string), typeof(ProgressBarWindow), new PropertyMetadata(default(string)));

    public string LabelTimeLeft
    {
        get => (string)GetValue(LabelTimeLeftProperty);
        set => SetValue(LabelTimeLeftProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LabelSpeedProperty = DependencyProperty.Register(nameof(LabelSpeed),
        typeof(string), typeof(ProgressBarWindow), new PropertyMetadata(default(string)));

    public string LabelSpeed
    {
        get => (string)GetValue(LabelSpeedProperty);
        set => SetValue(LabelSpeedProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(ProgressBarWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    #endregion

    private CancellationTokenSource? CancellationTokenSource { get; set; }
    private bool DownloadIsDone { get; set; }

    public ProgressBarWindow()
    {
        UpdateLanguage();

        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        this.SetWindowCornerPreference();
    }

    #region Action

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    /// <summary>
    /// Handles the actions to be performed when the ProgressBarWindow is closed.
    /// Cancels and disposes the cancellation token source if the download is not done.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private void ProgressBarWindow_OnClosed(object? sender, EventArgs e)
    {
        if (DownloadIsDone) return;

        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
    }

    #endregion

    #region Function

    /// <summary>
    /// Creates and starts a timer to update the estimated time left for the progress.
    /// </summary>
    /// <param name="timeLeftProgress">The progress reporter for the remaining time.</param>
    /// <returns>A Timer object that updates the remaining time.</returns>
    private Timer GetTimeLeftProgress(out IProgress<TimeSpan> timeLeftProgress)
    {
        var timeLeft = TimeSpan.Zero;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var timeLeftTimer = new Timer(TimeSpan.FromSeconds(1));
        timeLeftTimer.Elapsed += (_, _) => { Dispatcher.Invoke(() => { TimeLeftProgress = timeLeft; }); };
        timeLeftTimer.Start();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        timeLeftProgress = new Progress<TimeSpan>(d => { timeLeft = d; });
        return timeLeftTimer;
    }

    /// <summary>
    /// Initializes and returns a timer that updates the speed progress.
    /// </summary>
    /// <param name="speedProgress">The progress reporter for speed updates.</param>
    /// <returns>A Timer instance that updates the speed progress at regular intervals.</returns>
    private Timer GetSpeedProgress(out IProgress<(double NormalizeBytes, string NormalizeBytesUnit)> speedProgress)
    {
        double latestSpeed = 0;
        var latestUnit = string.Empty;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var speedTimer = new Timer(TimeSpan.FromSeconds(2.5));
        speedTimer.Elapsed += (_, _) =>
        {
            Dispatcher.Invoke(() =>
            {
                var roundNormalizeBytes = Math.Round(latestSpeed, 1);
                SpeedProgress = $"{roundNormalizeBytes} {latestUnit}/s";
            });
        };
        speedTimer.Start();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        speedProgress = new Progress<(double NormalizeBytes, string NormalizeBytesUnit)>(d =>
        {
            latestSpeed = d.NormalizeBytes;
            latestUnit = d.NormalizeBytesUnit;
        });
        return speedTimer;
    }

    /// <summary>
    /// Initiates the download process for a file with progress reporting.
    /// </summary>
    /// <param name="url">The URL of the file to be downloaded.</param>
    /// <param name="destinationFile">The local path where the downloaded file will be saved.</param>
    /// <param name="overwrite">If true, the existing file at the destination path will be overwritten if it exists.</param>
    /// <returns>A Task representing the asynchronous download operation.</returns>
    public async Task StartProgressBarDownload(string url, string destinationFile, bool overwrite = false)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var stopwatch = new Stopwatch();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        dispatcherTimer.Tick += (_, _) => Dispatcher.Invoke(() => { TimeElapsed = stopwatch.Elapsed; });

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        IProgress<double> percentProgress = new Progress<double>(d => { ProgressBarPercent.Value = d; });

        using var speedTimer = GetSpeedProgress(out var speedProgress);
        using var timeRemainingTimer = GetTimeLeftProgress(out var timeLeftProgress);

        CancellationTokenSource?.Dispose();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        CancellationTokenSource = new CancellationTokenSource();

        dispatcherTimer.Start();
        stopwatch.Start();
        await Http.DownloadFileWithReportAsync(url, destinationFile, overwrite, percentProgress: percentProgress,
            speedProgress: speedProgress, timeLeftProgress: timeLeftProgress,
            cancellationToken: CancellationTokenSource.Token);

        DownloadIsDone = true;
        speedTimer.Stop();
        dispatcherTimer.Stop();
        stopwatch.Stop();

        Thread.Sleep(TimeSpan.FromSeconds(2.5));
    }

    private void UpdateLanguage()
    {
        TitleWindow = ProgressBarWindowResources.TitleWindow;

        LabelTimeElapsed = ProgressBarWindowResources.LabelTimeElapsed;
        LabelTimeLeft = ProgressBarWindowResources.LabelTimeLeft;
        LabelSpeed = ProgressBarWindowResources.LabelSpeed;
    }

    #endregion
}