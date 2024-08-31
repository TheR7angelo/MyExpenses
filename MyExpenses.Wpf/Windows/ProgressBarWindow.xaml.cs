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

    public async Task StartProgressBarDownload(string url, string destinationFile, bool overwrite = false)
    {
        IProgress<double> percentProgress = new Progress<double>(d => { ProgressBarPercent.Value = d; });

        var speedTimer = GetSpeedProgress(out var speedProgress);

        CancellationTokenSource?.Dispose();
        CancellationTokenSource = new CancellationTokenSource();

        await Http.DownloadFileWithReportAsync(url, destinationFile, overwrite, percentProgress: percentProgress,
            speedProgress: speedProgress, cancellationToken: CancellationTokenSource.Token);

        DownloadIsDone = true;
        speedTimer.Stop();

        Thread.Sleep(TimeSpan.FromSeconds(2.5));
    }

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

    private void ProgressBarWindow_OnClosed(object? sender, EventArgs e)
    {
        if (DownloadIsDone) return;

        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
    }
}