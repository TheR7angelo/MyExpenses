using System.Windows;
using MyExpenses.WebApi;
using MyExpenses.Wpf.Utils;

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
        IProgress<(double NormalizeBytes, string NormalizeBytesUnit)> speedProgress =
            new Progress<(double NormalizeBytes, string NormalizeBytesUnit)>(d =>
            {
                var roundNormalizeBytes = Math.Round(d.NormalizeBytes);
                SpeedProgress = $"{roundNormalizeBytes} {d.NormalizeBytesUnit}/s";
            });

        CancellationTokenSource?.Dispose();
        CancellationTokenSource = new CancellationTokenSource();

        await Http.DownloadFileWithReportAsync(url, destinationFile, overwrite, percentProgress: percentProgress,
            speedProgress: speedProgress, cancellationToken: CancellationTokenSource.Token);

        DownloadIsDone = true;

        Thread.Sleep(TimeSpan.FromSeconds(2.5));
    }

    private void ProgressBarWindow_OnClosed(object? sender, EventArgs e)
    {
        if (DownloadIsDone) return;

        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
    }
}