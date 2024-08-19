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

    public static IEnumerable<T> ReadCsv<T>(this string filePath)
    {
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            DetectDelimiter = true
        };

        var encodingName = GetEncodingName(filePath);
        var encoding = Encoding.GetEncoding(encodingName);


        using var streamReader = new StreamReader(filePath, encoding, true,
            new FileStreamOptions { Access = FileAccess.Read, Share = FileShare.ReadWrite });

        using var reader = new CsvHelper.CsvReader(streamReader, csvConfiguration);
        var records = reader.GetRecords<T>();

        return records.ToList();
    }

    private static string GetEncodingName(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        var detector = new CharsetDetector();
        detector.Feed(fileStream);
        detector.DataEnd();

        var encodingName = string.IsNullOrEmpty(detector.Charset)
            ? "UTF-8"
            : detector.Charset;
        return encodingName;
    }
}