using System.Windows;
using MyExpenses.WebApi;
using MyExpenses.Wpf.Utils;
using Timer = System.Timers.Timer;

namespace MyExpenses.Wpf.Windows;

public partial class ProgressBarWindow
{
    #region DependencyProperty

    public static readonly DependencyProperty SpeedProgressProperty = DependencyProperty.Register(nameof(SpeedProgress),
        typeof(string), typeof(ProgressBarWindow), new PropertyMetadata(default(string)));

    public string SpeedProgress
    {
        get => (string)GetValue(SpeedProgressProperty);
        set => SetValue(SpeedProgressProperty, value);
    }

    #endregion

    private CancellationTokenSource? CancellationTokenSource { get; set; }
    private bool DownloadIsDone { get; set; }

    //TODO title
    public ProgressBarWindow()
    {
        InitializeComponent();

        this.SetWindowCornerPreference();
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
        IProgress<double> percentProgress = new Progress<double>(d => { ProgressBarPercent.Value = d; });

        using var speedTimer = GetSpeedProgress(out var speedProgress);

        CancellationTokenSource?.Dispose();
        CancellationTokenSource = new CancellationTokenSource();

        await Http.DownloadFileWithReportAsync(url, destinationFile, overwrite, percentProgress: percentProgress,
            speedProgress: speedProgress, cancellationToken: CancellationTokenSource.Token);

        DownloadIsDone = true;
        speedTimer.Stop();

        Thread.Sleep(TimeSpan.FromSeconds(2.5));
    }

    /// <summary>
    /// Initializes and returns a timer that updates the speed progress.
    /// </summary>
    /// <param name="speedProgress">The progress reporter for speed updates.</param>
    /// <returns>A Timer instance that updates the speed progress at regular intervals.</returns>
    private Timer GetSpeedProgress(out IProgress<(double NormalizeBytes, string NormalizeBytesUnit)> speedProgress)
    {
        double latestSpeed = 0;
        var latestUnit = "";
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

        speedProgress = new Progress<(double NormalizeBytes, string NormalizeBytesUnit)>(d =>
        {
            latestSpeed = d.NormalizeBytes;
            latestUnit = d.NormalizeBytesUnit;
        });
        return speedTimer;
    }

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
}