using System.Globalization;
using Serilog;

namespace MyExpenses.WebApi;

public abstract class Http
{
    protected internal static HttpClient GetHttpClient(string? baseUrl=null, string? userAgent=null)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The HttpClient instance is created here to configure and build the HttpClient.
        // Since it is a scoped and lightweight object, its allocation occurs only when required and has no measurable impact on performance.
        var httpClient = new HttpClient();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The Uri instance is created here to build the base address of the HttpClient.
        // Since it is a scoped and lightweight object, its allocation occurs only when required and has no measurable impact on performance.
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
    /// <param name="cancellationToken">Optional. A token that can be used to cancel the download. The default value is CancellationToken.None.</param>
    /// <exception cref="IOException">Thrown when the overwrite parameter is false and the destination file already exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file size can't be determined.</exception>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task DownloadFileWithReportAsync(string url, string destinationFile,
        bool overwrite = false, int logInterval = 5,
        IProgress<double>? percentProgress = null, IProgress<(double NormalizeBytes, string NormalizeBytesUnit)>? speedProgress = null,
        IProgress<TimeSpan>? timeLeftProgress = null, CancellationToken cancellationToken = default)
    {
        if (!overwrite && File.Exists(destinationFile))
        {
            throw new IOException($"Destination file {destinationFile} already exists.");
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The Progress instance is created here to report progress.
        // Since it is a scoped and lightweight object, its allocation occurs only when required and has no measurable impact on performance.
        IProgress<(double Percentage, double NormalizeBytes, string NormalizeBytesUnit, TimeSpan TimeLeft)> progressLog =
            new Progress<(double Percentage, double NormalizeBytes, string NormalizeBytesUnit, TimeSpan TimeLeft)>(
                value => Log.Information("Progress: {Percentage:F2}% | Speed: {Speed:F2} {NormalizeBytesUnit}/s | Time Left: {TimeLeft:g}",
                    value.Percentage.ToString(CultureInfo.InvariantCulture),
                    value.NormalizeBytes.ToString(CultureInfo.InvariantCulture),
                    value.NormalizeBytesUnit, value.TimeLeft.ToString()));

        try
        {
            using var httpClient = GetHttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? throw new InvalidOperationException("Unable to determine file size.");

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The MemoryStream instance is created here to read the response stream.
            // Since it is a scoped and lightweight object, its allocation occurs only when required and has no measurable impact on performance.
            var buffer = new byte[1024 * 1024]; // 1MB buffer

            await using var sourceStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var destinationStream = File.Create(destinationFile);

            var totalBytesRead = 0L;
            var startTime = DateTime.Now;
            var lastLogTime = DateTime.Now;
            int bytesRead;

            Log.Information("Starting to download file \"{Url}\" to \"{DestinationFile}\"", url, destinationFile);
            while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalBytesRead += bytesRead;

                // Calculate percentage, speed and time left
                var percentage = (double)totalBytesRead / totalBytes * 100;
                var duration = DateTime.Now - startTime;
                var bytesPerSecond = totalBytesRead / duration.TotalSeconds;
                var remainingBytes = totalBytes - totalBytesRead;
                var remainingTime = TimeSpan.FromSeconds(remainingBytes / bytesPerSecond);

                var normalizeBytes = GetNormalizeByteSize(bytesPerSecond, out var normalizeBytesUnit);

                // Update Progress
                percentProgress?.Report(percentage);
                speedProgress?.Report((normalizeBytes, normalizeBytesUnit));
                timeLeftProgress?.Report(remainingTime);

                var currentTime = DateTime.Now;
                if ((currentTime - lastLogTime).TotalSeconds >= logInterval)
                {
                    // Log Progress at intervals
                    progressLog?.Report((percentage, normalizeBytes, normalizeBytesUnit, remainingTime));
                    lastLogTime = currentTime;
                }

                // Check for cancellation
                cancellationToken.ThrowIfCancellationRequested();
            }

            var endTime = DateTime.Now;
            var totalDuration = endTime - startTime;

            // Log final details
            var normalizeBytesFinal = GetNormalizeByteSize(totalBytes, out var normalizeBytesUnitFinal);
            Log.Information(
                "Download completed successfully, start time: {StartTime:g} | end time: {EndTime:g} | total duration: {TotalDuration:g} | file size: {TotalSize:F2} {NormalizeBytesUnit}",
                startTime.ToString(CultureInfo.InvariantCulture),
                endTime.ToString(CultureInfo.InvariantCulture),
                totalDuration.ToString(), normalizeBytesFinal.ToString(CultureInfo.InvariantCulture), normalizeBytesUnitFinal);
        }
        catch (OperationCanceledException)
        {
            // Ensure the partially downloaded file is deleted if the operation is canceled
            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            Log.Information("Download was cancelled and the file \"{DestinationFile}\" was deleted", destinationFile);
            throw;
        }
    }

    private static double GetNormalizeByteSize(double bytes, out string unit)
    {
        var absoluteBytes = Math.Abs(bytes);

        string[] units = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

        var unitIndex = 0;
        while (absoluteBytes >= 1024 && unitIndex < units.Length - 1)
        {
            absoluteBytes /= 1024;
            ++unitIndex;
        }

        unit = units[unitIndex];
        return absoluteBytes;
    }
}