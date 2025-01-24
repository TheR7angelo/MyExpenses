using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using Ude;

namespace MyExpenses.IO.Csv;

public static class CsvReader
{
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
        // The allocations for FileStreamOptions, StreamReader, and CsvReader, as well as the CsvConfiguration instance,
        // are necessary for reading the file and parsing its content with the specified encoding and settings.
        // These objects are intentionally created within the scope of this method and wrapped in 'using' statements
        // to ensure that they are properly disposed of after use. Since these allocations are short-lived and limited
        // to this operation, they have no significant impact on the overall performance and enable efficient resource usage.
        var fileStreamOptions = new FileStreamOptions { Access = FileAccess.Read,Share = FileShare.ReadWrite };
        using var streamReader = new StreamReader(filePath, encoding, true, fileStreamOptions);

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            DetectDelimiter = true
        };

        using var reader = new CsvHelper.CsvReader(streamReader, csvConfiguration);

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