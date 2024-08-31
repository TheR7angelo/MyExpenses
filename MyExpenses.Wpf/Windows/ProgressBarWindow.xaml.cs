using MyExpenses.WebApi;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class ProgressBarWindow
{
    public ProgressBarWindow()
    {
        InitializeComponent();

        this.SetWindowCornerPreference();
    }

    public async Task StartProgressBarDownload(string url, string destinationFile, bool overwrite = false)
    {
        IProgress<double> percentProgress = new Progress<double>(d => { ProgressBarPercent.Value = d; });

        await Http.DownloadFileWithReportAsync(url, destinationFile, overwrite, percentProgress: percentProgress);

        Thread.Sleep(TimeSpan.FromSeconds(2.5));
    }

}