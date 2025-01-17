using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using Ude;

namespace MyExpenses.IO.Csv;

public static class CsvReader
{
    /// <summary>
    /// Provides the configuration options for CSV file operations used within the application.
    /// </summary>
    /// <remarks>
    /// This property is a static instance of <see cref="CsvHelper.Configuration.CsvConfiguration"/> preconfigured
    /// to use the invariant culture and to handle specific CSV parsing features. The configuration disables header
    /// validation and missing field handling while enabling automatic detection of the delimiter.
    /// </remarks>
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // The allocation here is intentional and necessary because CsvConfiguration requires instantiation with specific settings.
    // As this is a static property, the allocation happens only once during the lifetime of the application and has no significant impact on performance.
    private static CsvConfiguration CsvConfiguration { get; } = new(CultureInfo.InvariantCulture)
    {
        HeaderValidated = null,
        MissingFieldFound = null,
        DetectDelimiter = true
    };

    /// <summary>
    /// Provides configuration options for file stream operations, including access level,
    /// sharing mode, and other stream settings.
    /// </summary>
    /// <remarks>
    /// This property encapsulates a preconfigured instance of <see cref="System.IO.FileStreamOptions"/>
    /// designed for reading files with specific access and sharing permissions. It ensures compatibility
    /// when handling file streams in scenarios that require concurrent or shared file access.
    /// </remarks>
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // The allocation here is intentional and necessary to configure specific file stream options.
    // As this is a static property, the allocation occurs only once during the application lifetime and has no significant performance impact.
    private static FileStreamOptions FileStreamOptions { get; } = new()
    {
        Access = FileAccess.Read,
        Share = FileShare.ReadWrite
    };

    static CsvReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Reads the contents of a CSV file and maps them to a collection of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to which the CSV rows will be mapped.</typeparam>
    /// <param name="filePath">The file path of the CSV file to be read.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the mapped objects from the CSV file.</returns>
    public static IEnumerable<T> ReadCsv<T>(this string filePath)
    {
        var encodingName = GetEncodingName(filePath);
        var encoding = Encoding.GetEncoding(encodingName);

        // ReSharper disable HeapView.ObjectAllocation.Evident
        // The allocation of StreamReader and CsvReader is necessary here to read the file and parse its content according to the specified encoding and CSV configuration.
        // Both instances are properly wrapped in 'using' statements, ensuring they are disposed of immediately after use.
        // This short-lived allocation is required for the operation and does not impact overall performance.
        using var streamReader = new StreamReader(filePath, encoding, true, FileStreamOptions);
        using var reader = new CsvHelper.CsvReader(streamReader, CsvConfiguration);
        // ReSharper restore HeapView.ObjectAllocation.Evident

        var records = reader.GetRecords<T>();

        return records.ToList();
    }

    private static string GetEncodingName(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The allocation of CharsetDetector is required here, as it is used to process the file stream and determine the character set.
        // This allocation is minimal and short-lived, limited to the scope of this operation, and does not significantly impact performance.
        var detector = new CharsetDetector();
        detector.Feed(fileStream);
        detector.DataEnd();

        var encodingName = string.IsNullOrEmpty(detector.Charset)
            ? "UTF-8"
            : detector.Charset;
        return encodingName;
    }
}