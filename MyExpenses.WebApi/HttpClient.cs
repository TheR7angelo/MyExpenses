using Serilog;

namespace MyExpenses.WebApi;

public abstract class Http
{
    protected internal static HttpClient GetHttpClient(string? baseUrl=null, string? userAgent=null)
    {
        var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(baseUrl)) httpClient.BaseAddress = new Uri(baseUrl);

        userAgent ??= Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        return httpClient;
    }

    protected internal static string ParseToUrlFormat(string str) => str.Replace(" ", "+");

    /// <summary>
    /// Downloads a file asynchronously from the specified URL and saves it to the specified destination file.
    /// </summary>
    /// <param name="url">The URL of the file to download.</param>
    /// <param name="destinationFile">The destination file path where the downloaded file will be saved.</param>
    /// <param name="overwrite">Optional. Determines whether to overwrite the destination file if it already exists. The default value is false.</param>
    /// <param name="logInterval">Optional. The interval at which logs should be recorded in seconds. The default value is 5 seconds.</param>
    /// <param name="percentProgress">Optional. An object that reports the percentage progress of the download. The default value is null.</param>
    /// <param name="speedProgress">Optional. An object that reports the speed of the download in megabytes per second. The default value is null.</param>
    /// <param name="timeLeftProgress">Optional. An object that reports the estimated time remaining for the download to complete. The default value is null.</param>
    /// <exception cref="IOException">Thrown when the overwrite parameter is false and the destination file already exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file size can't be determined.</exception>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DownloadFileWithReportAsync(string url, string destinationFile,
        bool overwrite = false, int logInterval = 5,
        IProgress<double>? percentProgress = null, IProgress<double>? speedProgress = null,
        IProgress<TimeSpan>? timeLeftProgress = null)
    {
        if (!overwrite && File.Exists(destinationFile))
        {
            throw new IOException($"Destination file {destinationFile} already exists.");
        }

        IProgress<(double Percentage, double Speed, TimeSpan TimeLeft)> progressLog =
            new Progress<(double Percentage, double Speed, TimeSpan TimeLeft)>(
                value => Log.Information("Progress: {Percentage:F2}% | Speed: {Speed:F2} MB/s | Time Left: {TimeLeft:g}",
                    value.Percentage, value.Speed, value.TimeLeft));

        using var httpClient = GetHttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? throw new InvalidOperationException("Unable to determine file size.");
        var buffer = new byte[1024 * 1024]; // 1MB buffer

        await using var sourceStream = await response.Content.ReadAsStreamAsync();
        await using var destinationStream = File.Create(destinationFile);

        var totalBytesRead = 0L;
        var startTime = DateTime.Now;
        var lastLogTime = DateTime.Now;
        int bytesRead;

        while ((bytesRead = await sourceStream.ReadAsync(buffer)) > 0)
        {
            await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalBytesRead += bytesRead;

            // Calculate percentage, speed and time left
            var percentage = (double)totalBytesRead / totalBytes * 100;
            var duration = DateTime.Now - startTime;
            var bytesPerSecond = totalBytesRead / duration.TotalSeconds;
            var remainingBytes = totalBytes - totalBytesRead;
            var remainingTime = TimeSpan.FromSeconds(remainingBytes / bytesPerSecond);
            var speedInMBps = bytesPerSecond / (1024 * 1024);

            // Update Progress
            percentProgress?.Report(percentage);
            speedProgress?.Report(speedInMBps);
            timeLeftProgress?.Report(remainingTime);

            var currentTime = DateTime.Now;
            if (!((currentTime - lastLogTime).TotalSeconds >= logInterval)) continue;

            // Log Progress at intervals
            progressLog?.Report((percentage, speedInMBps, remainingTime));
            lastLogTime = currentTime;
        }

        var endTime = DateTime.Now;
        var totalDuration = endTime - startTime;

        // Log final details
        Log.Information("Download completed successfully, start time: {StartTime:g} | end time: {EndTime:g} | total duration: {TotalDuration:g}", startTime, endTime, totalDuration);
    }
}